using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

public class DialogueNode : DialogueNodeBase
{
    public readonly Vector2 DefaultNodeSize = new Vector2(192, 128);

    public string DialogueKey;
    public List<DialogueDecoratorBase> Decorators;

    VisualElement DecoratorArea;

    public DialogueNode()
    {

    }
    public DialogueNode(string nodeKey, Vector2 mousePosition)
    {
        title = "Dialogue Node";
        GUID = System.Guid.NewGuid().ToString();
        DialogueKey = nodeKey;
        Decorators = new List<DialogueDecoratorBase>();

        // Port Container
        {
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);

            var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);
        }

        // Port Division
        var portDivision = new VisualElement();
        portDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        portDivision.style.height = 1;
        mainContainer.Add(portDivision);

        DecoratorArea = new VisualElement();
        mainContainer.Add(DecoratorArea);

        // Key Area
        var keyArea = new VisualElement();
        keyArea.style.marginTop = 1;
        keyArea.style.marginBottom = 1;
        keyArea.style.flexDirection = FlexDirection.Row;
        keyArea.style.alignContent = Align.FlexEnd;
        // Key Area
        {
            var keyLabel = new Label("Key");
            keyLabel.style.marginLeft = 4;
            keyLabel.style.marginTop = 3;
            keyLabel.style.minWidth = 24;
            keyArea.Add(keyLabel);
        }

        // Key Text Field
        var keyTextField = new TextField(string.Empty);
        keyTextField.RegisterValueChangedCallback(evt => { DialogueKey = evt.newValue; });
        keyTextField.SetValueWithoutNotify(nodeKey);
        keyTextField.style.minWidth = 168;
        keyTextField.style.maxWidth = 168;
        keyTextField.style.flexDirection = FlexDirection.Column;
        keyTextField.style.whiteSpace = WhiteSpace.NoWrap;
        keyTextField.style.overflow = Overflow.Visible;
        keyArea.Add(keyTextField);
        mainContainer.Add(keyArea);

        // Text Area
        string translatedString = LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", DialogueKey);
        var textField = new TextField(string.Empty);
        textField.style.marginTop = 1;
        textField.style.marginBottom = 2;
        textField.style.maxWidth = 203;
        textField.multiline = true;
        textField.style.flexDirection = FlexDirection.Column;
        textField.style.whiteSpace = WhiteSpace.Normal;
        textField.style.flexGrow = 1;
        textField.style.flexShrink = 0;
        textField.style.overflow = Overflow.Hidden;
        textField.SetEnabled(false);
        textField.SetValueWithoutNotify(translatedString);
        mainContainer.Add(textField);

        // Text Callback
        keyTextField.RegisterValueChangedCallback(evt =>
        {
            string changedString = LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", DialogueKey);
            textField.SetValueWithoutNotify(changedString);
        });

        // Refresh
        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(mousePosition, DefaultNodeSize));
        mainContainer.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);

        Vector2 mousePosition = evt.mousePosition;
        if (evt.target is Node)
        {
            evt.menu.AppendAction("Add Decorator", (e) => { OpenDecoratorSearchWindow(mousePosition); });
        }
    }
    public void OpenDecoratorSearchWindow(Vector2 mousePosition)
    {
        DialogueDecoratorSearchWindow TargetSearchWindow = ScriptableObject.CreateInstance<DialogueDecoratorSearchWindow>();
        TargetSearchWindow.Init(this);
        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(mousePosition)), TargetSearchWindow);
    }
    public bool AddDecorator(DialogueDecoratorType type)
    {
        DialogueDecoratorBase decorator;
        switch (type)
        {
            case DialogueDecoratorType.Speech:
                decorator = new DialogueDecoratorSpeech(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.FrameCG:
                decorator = new DialogueDecoratorFrameCG(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.FullCG:
                decorator = new DialogueDecoratorFullCG(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.AnimationCG:
                decorator = new DialogueDecoratorAnimationCG(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.Transition:
                decorator = new DialogueDecoratorTransition(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.PlaySFX:
                decorator = new DialogueDecoratorPlaySFX(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.PlayMusic:
                decorator = new DialogueDecoratorPlayMusic(this);
                AddDecorator(decorator);
                return true;
            case DialogueDecoratorType.CameraShake:
                decorator = new DialogueDecoratorCameraShake(this);
                AddDecorator(decorator);
                return true;
            default:    
                return false;
        }

    }
    public DialogueDecoratorBase AddDecorator(DialogueDecoratorBase decorator)
    {
        DecoratorArea.Add(decorator);
        Decorators.Add(decorator);

        return decorator;
    }
    public void DeleteDecorator(DialogueDecoratorBase decorator)
    {
        Decorators.Remove(decorator);
        DecoratorArea.Remove(decorator);
    }
}

