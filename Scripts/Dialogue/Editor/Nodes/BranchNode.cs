using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.Localization.Settings;

public class BranchNode : DialogueNodeBase
{
    public readonly Vector2 DefaultNodeSize = new Vector2(192, 192);
    protected DialogueGraphView ParentGraphView;
    protected Dictionary<Port, VisualElement> portTextAreaPairs;

    public BranchNode(DialogueGraphView view, Vector2 mousePosition)
    {
        ParentGraphView = view;
        title = "Branch Node";
        GUID = System.Guid.NewGuid().ToString();

        style.width = 256;
        style.flexGrow = 0;
        style.flexShrink = 0;

        var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        inputPort.portName = "Input";
        inputContainer.Add(inputPort);

        var button = new Button(() => { AddBranchPort(); });
        button.text = "Add Branch";
        titleContainer.Add(button);

        RefreshExpandedState();
        RefreshPorts();
        
        // TextArea
        portTextAreaPairs = new Dictionary<Port, VisualElement>();
        
        SetPosition(new Rect(mousePosition, DefaultNodeSize));
        mainContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
    }

    public void AddBranchPort(string overriddenPortName = "")
    {
        var generatedPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        generatedPort.contentContainer.style.marginLeft = 0;
        generatedPort.contentContainer.style.marginRight = 0;

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        var branchPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Branch {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = branchPortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        textField.SetValueWithoutNotify(branchPortName);
        textField.style.width = 118;
        textField.style.flexGrow = 0;
        textField.style.flexShrink = 0;
        textField.style.marginLeft = 0;
        textField.style.marginRight = 0;
        generatedPort.portName = branchPortName;
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(generatedPort)) { text = "X" };
        deleteButton.style.height = 18;
        deleteButton.style.marginTop = 0;
        deleteButton.style.marginBottom = 0;
        generatedPort.Add(deleteButton);

        outputContainer.Add(generatedPort);
        
        // Text Area
        var translatedString = LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", generatedPort.portName);
        var textFieldEx = new TextField(string.Empty);
        textFieldEx.style.marginTop = 1;
        textFieldEx.style.marginBottom = 2;
        textFieldEx.style.maxWidth = 256;
        textFieldEx.multiline = true;
        textFieldEx.style.flexDirection = FlexDirection.Column;
        textFieldEx.style.whiteSpace = WhiteSpace.Normal;
        textFieldEx.style.flexGrow = 1;
        textFieldEx.style.flexShrink = 0;
        textFieldEx.style.overflow = Overflow.Hidden;
        textFieldEx.SetEnabled(false);
        textFieldEx.SetValueWithoutNotify(translatedString);
        mainContainer.Add(textFieldEx);
        portTextAreaPairs.Add(generatedPort, textFieldEx);
        
        RefreshExpandedState();
        RefreshPorts();
    }
    private void RemovePort(Port generatedPort)
    {
        var targetEdge = ParentGraphView.edges.ToList().Where(x => x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            ParentGraphView.RemoveElement(targetEdge.First());
        }

        outputContainer.Remove(generatedPort);
        RefreshPorts();
        RefreshExpandedState();
    }
}

