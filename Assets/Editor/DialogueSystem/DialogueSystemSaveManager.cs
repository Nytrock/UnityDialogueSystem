using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public static class DialogueSystemSaveManager {
    private static DialogueSystemGraphView _graphView;

    private static string _graphFileName;
    private static string _graphFolderPath;

    private static List<DialogueSystemGroup> _groups;
    private static List<DialogueBaseNode> _nodes;
    private static Dictionary<string, DialogueGroup> _createdDialogueGroups;
    private static Dictionary<string, Dialogue> _createdDialogues;

    private static Dictionary<string, DialogueSystemGroup> _loadedGroups;
    private static Dictionary<string, DialogueBaseNode> _loadedNodes;

    public static void Initialize(DialogueSystemGraphView graphView, string graphName) {
        _graphView = graphView;
        _graphFileName = graphName;
        _graphFolderPath = $"Assets/_Project/ScriptableObjects/Dialogues/{graphName}";

        _groups = new();
        _nodes = new();
        _createdDialogueGroups = new();
        _createdDialogues = new();
        _loadedGroups = new();
        _loadedNodes = new();
    }

    #region Save
    public static void Save() {
        CreateStaticFolders();
        GetElementsFromGraphView();

        DialogueSystemGraphSaveData graphData = AssetsUtility.CreateAsset<DialogueSystemGraphSaveData>("Assets/_Project/Editor/DialogueSystem/Graphs", _graphFileName);
        graphData.Initialize(_graphFileName);

        DialogueContainer dialogueContainer = AssetsUtility.CreateAsset<DialogueContainer>(_graphFolderPath, _graphFileName);
        dialogueContainer.Initialize(_graphFileName);

        SaveGroups(graphData, dialogueContainer);
        SaveNodes(graphData, dialogueContainer);

        graphData.Save();
        dialogueContainer.Save();
    }

    #region Groups
    private static void SaveGroups(DialogueSystemGraphSaveData graphData, DialogueContainer dialogueContainer) {
        List<string> groupNames = new();
        foreach (var group in _groups) {
            SaveGroupToGraph(group, graphData);
            SaveGroupToScriptableObject(group, dialogueContainer);
            groupNames.Add(group.title);
        }

        UpdateOldGroups(groupNames, graphData);
    }

    private static void SaveGroupToGraph(DialogueSystemGroup group, DialogueSystemGraphSaveData graphData) {
        DialogueGroupSaveData groupData = new(
            group.ID,
            group.title,
            group.GetPosition().position
        );

        graphData.AddGroup(groupData);
    }

    private static void SaveGroupToScriptableObject(DialogueSystemGroup group, DialogueContainer dialogueContainer) {
        string groupName = group.title;
        FoldersUtility.CreateEditorFolder($"{_graphFolderPath}/Groups", groupName);
        FoldersUtility.CreateEditorFolder($"{_graphFolderPath}/Groups/{groupName}", "Dialogues");

        DialogueGroup dialogueGroup = AssetsUtility.CreateAsset<DialogueGroup>($"{_graphFolderPath}/Groups/{groupName}", groupName);
        dialogueGroup.Initialize(groupName);
        dialogueContainer.AddGroup(dialogueGroup);
        _createdDialogueGroups.Add(group.ID, dialogueGroup);

        dialogueGroup.Save();
    }

    private static void UpdateOldGroups(List<string> currentGroupNames, DialogueSystemGraphSaveData graphData) {
        foreach (var oldName in graphData.OldGroupNames) {
            if (currentGroupNames.Contains(oldName))
                continue;

            FoldersUtility.DeleteEditorFolder($"{_graphFolderPath}/Groups/{oldName}");
        }

        graphData.UpdateOldGroupNames(new(currentGroupNames));
    }
    #endregion

    #region Nodes
    private static void SaveNodes(DialogueSystemGraphSaveData graphData, DialogueContainer dialogueContainer) {
        List<string> nodeNames = new();
        SerializableDictionary<string, List<string>> groupedNodeNames = new();

        foreach (var node in _nodes) {
            SaveNodeToGraph(node, graphData);
            SaveNodeToScriptableObject(node, dialogueContainer);
            if (node.Group != null) {
                if (groupedNodeNames.ContainsKey(node.Group.ID))
                    groupedNodeNames[node.Group.ID].Add(node.DialogueName);
                else
                    groupedNodeNames[node.Group.ID] = new() { node.DialogueName };
                continue;
            }

            nodeNames.Add(node.DialogueName);
        }

        UpdateDialoguesChoicesConnections();
        UpdateOldGroupedNodes(groupedNodeNames, graphData);
        UpdateOldUngroupedNodes(nodeNames, graphData);
    }

    private static void SaveNodeToGraph(DialogueBaseNode node, DialogueSystemGraphSaveData graphData) {
        List<DialogueChoiceSaveData> choices = CloneNodeChoices(node.Choices);

        DialogueNodeSaveData nodeData = new(
            node.ID,
            node.DialogueName,
            node.Text,
            choices,
            node.Group?.ID,
            node.DialogueType,
            node.GetPosition().position,
            node.Character,
            node.Emotion
        );

        graphData.AddNode(nodeData);
    }

    private static List<DialogueChoiceSaveData> CloneNodeChoices(IEnumerable<DialogueChoiceSaveData> originalChoices) {
        List<DialogueChoiceSaveData> choices = new();
        foreach (var choice in originalChoices)
            choices.Add(choice.Copy());
        return choices;
    }

    private static void SaveNodeToScriptableObject(DialogueBaseNode node, DialogueContainer dialogueContainer) {
        Dialogue dialogue;
        if (node.Group != null) {
            dialogue = AssetsUtility.CreateAsset<Dialogue>($"{_graphFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
            dialogueContainer.AddGroupDialogue(_createdDialogueGroups[node.Group.ID], dialogue);
        } else {
            dialogue = AssetsUtility.CreateAsset<Dialogue>($"{_graphFolderPath}/Global/Dialogues", node.DialogueName);
            dialogueContainer.AddUngroupDialogue(dialogue);
        }

        dialogue.Initialize(
            node.DialogueName,
            node.Text,
            ConvertNodeChoicesToDialogueChoices(node.Choices),
            node.DialogueType,
            node.Character,
            node.Emotion,
            node.IsStartingNode()
        );
        _createdDialogues.Add(node.ID, dialogue);

        dialogue.Save();
    }

    private static void UpdateDialoguesChoicesConnections() {
        foreach (var node in _nodes) {
            Dialogue dialogue = _createdDialogues[node.ID];

            for (int i = 0; i < node.Choices.Count(); i++) {
                DialogueChoiceSaveData nodeChoice = node.GetChoice(i);
                if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    continue;

                dialogue.SetChoiceNextDialogue(_createdDialogues[nodeChoice.NodeID], i);
                dialogue.Save();
            }
        }
    }

    private static List<DialogueChoiceData> ConvertNodeChoicesToDialogueChoices(IEnumerable<DialogueChoiceSaveData> nodeChoices) {
        List<DialogueChoiceData> dialogueChoices = new();
        foreach (var nodeChoice in nodeChoices)
            dialogueChoices.Add(nodeChoice.ToDialogueChoice());
        return dialogueChoices;
    }

    private static void UpdateOldUngroupedNodes(List<string> currentNodeNames, DialogueSystemGraphSaveData graphData) {
        foreach (var oldName in graphData.OldUngroupedNodeNames) {
            if (currentNodeNames.Contains(oldName))
                continue;

            AssetsUtility.RemoveAsset($"{_graphFolderPath}/Global/Dialogues", oldName);
        }

        graphData.UpdateOldUngroupedNodeNames(new(currentNodeNames));
    }

    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DialogueSystemGraphSaveData graphData) {
        foreach (var oldGroupedNode in graphData.OldGroupedNodeNames) {
            if (!currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                continue;

            foreach (var groupedNode in oldGroupedNode.Value) {
                if (currentGroupedNodeNames[oldGroupedNode.Key].Contains(groupedNode))
                    continue;

                AssetsUtility.RemoveAsset($"{_graphFolderPath}/Global/{oldGroupedNode.Key}/Dialogues", groupedNode);
            }
        }

        graphData.UpdateOldGroupedNodeNames(new(currentGroupedNodeNames));
    }
    #endregion

    private static void GetElementsFromGraphView() {
        _graphView.graphElements.ForEach(graphElement => {
            if (graphElement is DialogueBaseNode node)
                _nodes.Add(node);
            else if (graphElement is DialogueSystemGroup group)
                _groups.Add(group);
        });
    }
    #endregion

    #region Load
    public static void Load() {
        DialogueSystemGraphSaveData graphData = AssetsUtility.LoadAsset<DialogueSystemGraphSaveData>("Assets/_Project/Editor/DialogueSystem/Graphs", _graphFileName);
        if (graphData == null) {
            EditorUtility.DisplayDialog(
                "Cannot load the file!",
                $"File {_graphFileName}.asset cannot be found. Change name and try again.",
                "Ok"
            );
            return;
        }

        DialogueSystemEditorWindow.UpdateFileName(graphData.FileName);

        LoadGroups(graphData);
        LoadNodes(graphData);
        LoadNodesConnections();
    }

    private static void LoadGroups(DialogueSystemGraphSaveData graphData) {
        foreach (var groupData in graphData.Groups) {
            DialogueSystemGroup group = _graphView.CreateGroup(groupData.Name, groupData.Position);
            group.SetID(groupData.ID);
            _loadedGroups.Add(groupData.ID, group);
        }
    }

    private static void LoadNodes(DialogueSystemGraphSaveData graphData) {
        foreach (var nodeData in graphData.Nodes) {
            List<DialogueChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);
            DialogueBaseNode node = _graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);
            node.Setup(nodeData, choices);
            node.Draw();

            _loadedNodes.Add(node.ID, node);

            if (string.IsNullOrEmpty(nodeData.GroupID))
                continue;

            DialogueSystemGroup group = _loadedGroups[nodeData.GroupID];
            node.ChangeGroup(group);
            group.AddElement(node);
        }
    }

    private static void LoadNodesConnections() {
        foreach (var loadedNode in _loadedNodes) {
            foreach (var element in loadedNode.Value.outputContainer.Children()) {
                if (element is not Port choicePort)
                    continue;

                DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)choicePort.userData;
                if (string.IsNullOrEmpty(choiceData.NodeID))
                    continue;

                DialogueBaseNode nextNode = _loadedNodes[choiceData.NodeID];
                Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();
                _graphView.AddElement(choicePort.ConnectTo(nextNodeInputPort));
            }
            loadedNode.Value.RefreshPorts();
        }
    }
    #endregion

    private static void CreateStaticFolders() {
        FoldersUtility.CreateEditorFolder("Assets/_Project/Editor/DialogueSystem", "Graphs");
        FoldersUtility.CreateEditorFolder("Assets", "ScriptableObjects");
        FoldersUtility.CreateEditorFolder("Assets/_Project/ScriptableObjects", "Dialogues"); ;
        FoldersUtility.CreateEditorFolder("Assets/_Project/ScriptableObjects/Dialogues", _graphFileName); ;

        FoldersUtility.CreateEditorFolder(_graphFolderPath, "Global");
        FoldersUtility.CreateEditorFolder(_graphFolderPath, "Groups");
        FoldersUtility.CreateEditorFolder($"{_graphFolderPath}/Global", "Dialogues");
    }
}
