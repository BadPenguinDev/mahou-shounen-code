using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class ExitPointNode : DialogueNodeBase
{
    public readonly Vector2 DefaultNodePosition = new Vector2(768, 48);
    public readonly Vector2 DefaultNodeSize = new Vector2(192, 192);

    public ExitPointNode()
    {
        InitializeExitPointNode();
        SetPosition(new Rect(DefaultNodePosition, DefaultNodeSize));
    }
    public ExitPointNode(Vector2 position)
    {
        InitializeExitPointNode();
        SetPosition(new Rect(position, DefaultNodeSize));
    }

    protected void InitializeExitPointNode()
    {
        title = "Exit";
        GUID = "ExitPoint";

        var generatedPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
        generatedPort.portName = "End";
        inputContainer.Add(generatedPort);

        capabilities &= ~Capabilities.Deletable;

        RefreshExpandedState();
        RefreshPorts();
    }
    public void SetPosition(Vector2 position)
    {
        SetPosition(new Rect(position, DefaultNodeSize));
    }
}
