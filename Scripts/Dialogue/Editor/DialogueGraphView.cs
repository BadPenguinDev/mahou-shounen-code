using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public enum DialogueNodeType { Dialogue, Branch };
public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(128, 192);
    private DialogueNodeSearchWindow NodeSearchWindow;

    // Initializer
    public DialogueGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddElement(GenerateExitPointNode());

        // Add Search Window
        NodeSearchWindow = ScriptableObject.CreateInstance<DialogueNodeSearchWindow>();
        NodeSearchWindow.Init(editorWindow, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), NodeSearchWindow);
    }

    // Nodes
    public void CreateNode(DialogueNodeType type, string nodeKey, Vector2 mousePosition)
    {
        switch (type)
        {
            case DialogueNodeType.Dialogue:
                AddElement(CreateDialogueNode(nodeKey, mousePosition));
                break;
            case DialogueNodeType.Branch:
                AddElement(CreateBranchNode(mousePosition));
                break;
        }
    }
    public DialogueNode CreateDialogueNode(string nodeKey, Vector2 mousePosition)
    {
        var dialogueNode = new DialogueNode(nodeKey, mousePosition);
        return dialogueNode;
    }
    public BranchNode CreateBranchNode(Vector2 mousePosition)
    {
        var branchNode = new BranchNode(this, mousePosition);
        return branchNode;
    }
    private EntryPointNode GenerateEntryPointNode()
    {
        var entryPointNode = new EntryPointNode();
        entryPointNode.SetPosition(new Rect(64, 48, 192, 128));
        return entryPointNode;
    }
    public ExitPointNode GenerateExitPointNode()
    {
        var exitPointNode = new ExitPointNode();
        exitPointNode.SetPosition(new Rect(768, 48, 192, 128));
        return exitPointNode;
    }

    // Ports
    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePort = new List<Port>();

        ports.ForEach((port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePort.Add(port);
        }));

        return compatiblePort;
    }
    public void AddBranchPort(DialogueNode dialogueNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var branchPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Branch {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = branchPortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.Add(deleteButton);
        generatedPort.portName = branchPortName;

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }
    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
