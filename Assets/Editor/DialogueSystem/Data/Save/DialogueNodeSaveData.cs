using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNodeSaveData {
    [SerializeField] private string _ID;
    [SerializeField] private string _name;
    [SerializeField] private string _text;
    [SerializeField] private List<DialogueChoiceSaveData> _choices;
    [SerializeField] private string _groupID;
    [SerializeField] private DialogueType _dialogueType;
    [SerializeField] private Vector2 _position;
    [SerializeField] private DialogueCharacter _character;
    [SerializeField] private DialogueCharacterEmotion _emotion;

    public string ID => _ID;
    public string Name => _name;
    public string Text => _text;
    public IEnumerable<DialogueChoiceSaveData> Choices => _choices;
    public string GroupID => _groupID;
    public DialogueType DialogueType => _dialogueType;
    public Vector2 Position => _position;
    public DialogueCharacter Character => _character;
    public DialogueCharacterEmotion Emotion => _emotion;

    public DialogueNodeSaveData(string id, string name, string text, List<DialogueChoiceSaveData> choices, string groupID, DialogueType dialogueType, Vector2 position, DialogueCharacter character, DialogueCharacterEmotion emotion) {
        _ID = id;
        _name = name;
        _text = text;
        _choices = choices;
        _groupID = groupID;
        _dialogueType = dialogueType;
        _position = position;
        _character = character;
        _emotion = emotion;
    }
}
