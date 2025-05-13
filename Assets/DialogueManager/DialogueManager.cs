using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour {
    [SerializeField] private GameObject _panel;
    [SerializeField] private DialogueRenderer _renderer;
    private Dialogue _nowDialogue;

    public event Action DialogueEnded;

    private void Awake() {
        _panel.SetActive(false);
    }

    public void StartDialogue(DialogueChoicer dialogueChoicer) {
        _panel.SetActive(true);
        SetDialogue(dialogueChoicer.Dialogue);
    }

    private void Update() {
        if (_nowDialogue == null)
            return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            OnKeyPressed();
    }

    private void OnKeyPressed() {
        if (_renderer.IsAnimated)
            _renderer.StopTextAnimation();
        else
            SetDialogue(_nowDialogue.GetNextDialogue());
    }

    private void SetDialogue(Dialogue dialogue) {
        _nowDialogue = dialogue;
        if (dialogue == null) {
            StopDialogue();
            return;
        }

        _renderer.RenderDialogue(_nowDialogue);
    }

    private void StopDialogue() {
        _panel.SetActive(false);
        DialogueEnded?.Invoke();
    }
}
