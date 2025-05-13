using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ScriptableObject {
    [SerializeField] private string _name;
    [SerializeField, TextArea] private string _text;
    [SerializeField] private List<DialogueChoiceData> _choices;
    [SerializeField] private DialogueType _type;
    [SerializeField] private bool _isStartingDialogue;
    [SerializeField] private DialogueCharacter _character;
    [SerializeField] private DialogueCharacterEmotion _emotion;

    public string Name => _name;
    public string Text => _text;
    public DialogueType Type => _type;
    public DialogueCharacter Character => _character;
    public DialogueCharacterEmotion Emotion => _emotion;
    public bool IsStartingDialogue => _isStartingDialogue;

    public void Initialize(string name, string text, List<DialogueChoiceData> choices, DialogueType type, DialogueCharacter character, DialogueCharacterEmotion emotion, bool isStartingDialogue) {
        _name = name;
        _text = text;
        _choices = choices;
        _type = type;
        _isStartingDialogue = isStartingDialogue;
        _character = character;
        _emotion = emotion;
    }

    public void SetChoiceNextDialogue(Dialogue nextDialogue, int index) {
        _choices[index].SetNextDialogue(nextDialogue);
    }

    public Dialogue GetNextDialogue() {
        foreach (var choice in _choices)
            if (choice.NextDialogue != null)
                return choice.NextDialogue;
        return null;
    }
}
