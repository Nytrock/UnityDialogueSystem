using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueMultipleChoiceNode : DialogueBaseNode {
    protected override DialogueType _type => DialogueType.MultipleChoice;

    public override void Initialize(string nodeName, DialogueSystemGraphView graphView, Vector2 position) {
        base.Initialize(nodeName, graphView, position);
        DialogueChoiceSaveData choice = new("New Choice");
        _choices.Add(choice);
    }

    public override void Draw() {
        base.Draw();

        Button addChoiceButton = UIElementUtility.CreateButton("Add Choice", delegate {
            DialogueChoiceSaveData choice = new("New Choice");
            outputContainer.Add(CreateChoicePort(choice));
            _choices.Add(choice);
        });
        addChoiceButton.AddToClassList("ds-node__button");
        mainContainer.Insert(1, addChoiceButton);
    }

    protected override Port CreateChoicePort(object userData) {
        Port choicePort = this.CreatePort();
        choicePort.userData = userData;

        DialogueChoiceSaveData choiceData = (DialogueChoiceSaveData)userData;

        Button deleteChoiceButton = UIElementUtility.CreateButton("X", () => {
            if (_choices.Count == 1)
                return;

            if (choicePort.connected)
                _graphView.DeleteElements(choicePort.connections);

            _choices.Remove(choiceData);
            _graphView.RemoveElement(choicePort);
        });
        deleteChoiceButton.AddToClassList("ds-node__button");

        TextField textChoiceField = UIElementUtility.CreateTextField(choiceData.Text, onValueChanged: callback => {
            choiceData.SetText(callback.newValue);
        });
        textChoiceField.AddClasses(
            "ds-node__text-field",
            "ds-node__text-field__hidden",
            "ds-node__choice-text-field"
        );

        choicePort.Add(textChoiceField);
        choicePort.Add(deleteChoiceButton);
        return choicePort;
    }
}
