using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class EntryPointNode : DialogueNodeBase
{
    public EntryPointNode()
    {
        title = "Start";
        GUID = "EntryPoint";

        var generatedPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        generatedPort.portName = "Next";
        outputContainer.Add(generatedPort);

        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;

        RefreshExpandedState();
        RefreshPorts();

        SetPosition(new Rect(64, 48, 192, 128));
    }
}
