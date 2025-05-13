using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using NodeDirection = UnityEditor.Experimental.GraphView.Direction;

public static class UIElementUtility {
    public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) {
        TextField textField = new() {
            value = value,
            label = label,
        };

        if (onValueChanged != null)
            textField.RegisterValueChangedCallback(onValueChanged);
        return textField;
    }

    public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) {
        TextField textArea = CreateTextField(value, label, onValueChanged);
        textArea.multiline = true;
        return textArea;
    }

    public static Foldout CreateFoldout(string title, bool collapsed = false) {
        Foldout foldout = new() {
            text = title,
            value = !collapsed
        };

        return foldout;
    }

    public static Button CreateButton(string text, Action onClick = null) {
        Button button = new(onClick) {
            text = text,
        };
        return button;
    }


    public static Port CreatePort(this DialogueBaseNode node, string portName = "", Orientation orientation = Orientation.Horizontal, NodeDirection direction = NodeDirection.Output, Port.Capacity capacity = Port.Capacity.Single) {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(object));
        port.portName = portName;
        return port;
    }

    public static ObjectField CreateObjectField(string title, Type type, UnityEngine.Object value = null, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null) {
        ObjectField objectField = new() {
            objectType = type,
            label = title,
            value = value,
        };

        if (onValueChanged != null)
            objectField.RegisterValueChangedCallback(onValueChanged);
        return objectField;
    }

    public static EnumField CreateEnumField(string title, Enum type, EventCallback<ChangeEvent<Enum>> onValueChanged = null) {
        EnumField enumField = new(title, type);
        if (onValueChanged != null)
            enumField.RegisterValueChangedCallback(onValueChanged);
        return enumField;
    }
}
