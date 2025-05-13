using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueSystemEditorWindow : EditorWindow {
    private DialogueSystemGraphView _graphView;
    private readonly string _defaultFileName = "DialogueName";
    private static TextField _fileNameField;
    private Button _saveButton;
    private Button _minimapButton;

    [MenuItem("Window/Dialogue System/Dialogue Graph")]
    public static void ShowExample() {
        GetWindow<DialogueSystemEditorWindow>("Dialogue Graph");
    }

    private void CreateGUI() {
        AddGraphView();
        AddToolbar();

        AddStyles();
    }

    private void AddGraphView() {
        _graphView = new(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void AddToolbar() {
        Toolbar toolbar = new();
        toolbar.AddStyleSheets("DialogueSystem/ToolbarStyles.uss");

        _fileNameField = UIElementUtility.CreateTextField(_defaultFileName, "File Name: ", callback => {
            _fileNameField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
        });
        toolbar.Add(_fileNameField);

        _saveButton = UIElementUtility.CreateButton("Save", Save);
        toolbar.Add(_saveButton);

        Button loadButton = UIElementUtility.CreateButton("Load", Load);
        toolbar.Add(loadButton);

        Button clearButton = UIElementUtility.CreateButton("Clear", _graphView.ClearGraph);
        toolbar.Add(clearButton);

        Button resetButton = UIElementUtility.CreateButton("Reset", ResetGraph);
        toolbar.Add(resetButton);

        _minimapButton = UIElementUtility.CreateButton("Minimap", ChangeMinimapState);
        toolbar.Add(_minimapButton);

        rootVisualElement.Add(toolbar);
    }

    private void ResetGraph() {
        _graphView.ClearGraph();
        UpdateFileName(_defaultFileName);
    }

    private void Save() {
        if (string.IsNullOrEmpty(_fileNameField.value)) {
            EditorUtility.DisplayDialog("Invalid file name", "Change it and try again", "Ok");
            return;
        }


        DialogueSystemSaveManager.Initialize(_graphView, _fileNameField.value);
        DialogueSystemSaveManager.Save();
    }

    private void Load() {
        string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/_Project/Editor/DialogueSystem/Graphs", "asset");
        if (string.IsNullOrEmpty(filePath))
            return;

        _graphView.ClearGraph();
        DialogueSystemSaveManager.Initialize(_graphView, Path.GetFileNameWithoutExtension(filePath));
        DialogueSystemSaveManager.Load();
    }

    private void AddStyles() {
        rootVisualElement.AddStyleSheets("DialogueSystem/Variables.uss");
    }

    public void ChangeSaveButtonState(bool newStae) {
        _saveButton.SetEnabled(newStae);
    }

    private void ChangeMinimapState() {
        _graphView.ChangeMinimapState();
        _minimapButton.ToggleInClassList("ds-toolbar__button__selected");
    }

    public static void UpdateFileName(string fileName) {
        _fileNameField.value = fileName;
    }
}
