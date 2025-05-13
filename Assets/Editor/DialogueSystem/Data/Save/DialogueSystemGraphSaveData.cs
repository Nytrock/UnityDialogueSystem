using System.Collections.Generic;
using UnityEngine;

public class DialogueSystemGraphSaveData : ScriptableObject {
    [SerializeField] private string _fileName;
    [SerializeField] private List<DialogueGroupSaveData> _groups;
    [SerializeField] private List<DialogueNodeSaveData> _nodes;
    [SerializeField] private List<string> _oldGroupNames;
    [SerializeField] private List<string> _oldUngroupedNodeNames;
    [SerializeField] private SerializableDictionary<string, List<string>> _oldGroupedNodeNames;

    public string FileName => _fileName;
    public IEnumerable<DialogueGroupSaveData> Groups => _groups;
    public IEnumerable<DialogueNodeSaveData> Nodes => _nodes;
    public IEnumerable<string> OldGroupNames => _oldGroupNames;
    public IEnumerable<KeyValuePair<string, List<string>>> OldGroupedNodeNames => _oldGroupedNodeNames;
    public IEnumerable<string> OldUngroupedNodeNames => _oldUngroupedNodeNames;

    public void Initialize(string fileName) {
        _fileName = fileName;
        _groups = new();
        _nodes = new();
        _oldGroupNames = new();
        _oldUngroupedNodeNames = new();
        _oldGroupedNodeNames = new();
    }

    public void AddGroup(DialogueGroupSaveData groupData) {
        _groups.Add(groupData);
    }

    public void AddNode(DialogueNodeSaveData nodeData) {
        _nodes.Add(nodeData);
    }

    public void UpdateOldGroupNames(List<string> newNames) {
        _oldGroupNames = newNames;
    }

    public void UpdateOldUngroupedNodeNames(List<string> newNames) {
        _oldUngroupedNodeNames = newNames;
    }

    public void UpdateOldGroupedNodeNames(SerializableDictionary<string, List<string>> newNames) {
        _oldGroupedNodeNames = newNames;
    }
}
