using System;
using UnityEngine;

[Serializable]
public class DialogueChoiceData {
    [SerializeField] private string _text;
    [SerializeField] private Dialogue _nextDialogue;

    public Dialogue NextDialogue => _nextDialogue;

    public DialogueChoiceData(string text) {
        _text = text;
    }

    public void SetNextDialogue(Dialogue nextDialogue) {
        _nextDialogue = nextDialogue;
    }
}
