using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueDecoratorSpeech : DialogueDecoratorBase
{
    public PortraitType PortraitType;
    public EmotionType EmotionType;
    public PortraitOrientation PortraitOrientation;

    EnumField EnumFieldPortrait;
    EnumField EnumFieldEmotion;

    Toggle ToggleLeft;
    Toggle ToggleRight;

    public DialogueDecoratorSpeech(DialogueNode node)
    {
        VisualElement decoratorBase = new VisualElement();
        decoratorBase.style.marginTop = 1;
        decoratorBase.style.flexDirection = FlexDirection.Row;
        Add(decoratorBase);
        // DecoratorBase
        {
            Button buttonDelete = new Button(() => { node.DeleteDecorator(this); });
            buttonDelete.text = "X";
            buttonDelete.style.marginLeft = 3;
            buttonDelete.style.marginRight = 0;
            buttonDelete.style.marginTop = 1;
            buttonDelete.style.marginBottom = 1;
            decoratorBase.Add(buttonDelete);

            VisualElement decoratorField = new VisualElement();
            decoratorField.style.flexDirection = FlexDirection.Column;
            decoratorBase.Add(decoratorField);
            // Decorator Field
            {
                VisualElement decoratorFieldTop = new VisualElement();
                decoratorFieldTop.style.flexDirection = FlexDirection.Row;
                decoratorField.Add(decoratorFieldTop);
                // Decorator Field Top
                {
                    EnumFieldPortrait = new EnumField();
                    EnumFieldPortrait.Init(PortraitType);
                    EnumFieldPortrait.RegisterValueChangedCallback((value) => { PortraitType = (PortraitType)value.newValue; });
                    EnumFieldPortrait.style.height = 18;
                    EnumFieldPortrait.style.marginLeft = 2;
                    EnumFieldPortrait.style.marginRight = 0;
                    EnumFieldPortrait.style.minWidth = 120;
                    EnumFieldPortrait.style.maxWidth = 120;
                    decoratorFieldTop.Add(EnumFieldPortrait);

                    ToggleLeft = new Toggle();
                    ToggleLeft.RegisterValueChangedCallback((value) => { SetPortraitOrientation(PortraitOrientation.Left); });
                    ToggleLeft.SetValueWithoutNotify(true);
                    ToggleLeft.text = "Left";
                    ToggleLeft.style.top = 1;
                    ToggleLeft.style.marginLeft = 0;
                    ToggleLeft.style.minWidth = 42;
                    decoratorFieldTop.Add(ToggleLeft);
                }

                VisualElement decoratorFieldBottom = new VisualElement();
                decoratorFieldBottom.style.flexDirection = FlexDirection.Row;
                decoratorField.Add(decoratorFieldBottom);
                // Decorator Field Top
                {
                    EnumFieldEmotion = new EnumField();
                    EnumFieldEmotion.Init(EmotionType);
                    EnumFieldEmotion.RegisterValueChangedCallback((value) => { EmotionType = (EmotionType)value.newValue; });
                    EnumFieldEmotion.style.height = 18;
                    EnumFieldEmotion.style.marginLeft = 2;
                    EnumFieldEmotion.style.marginRight = 0;
                    EnumFieldEmotion.style.minWidth = 120;
                    EnumFieldEmotion.style.maxWidth = 120;
                    decoratorFieldBottom.Add(EnumFieldEmotion);

                    ToggleRight = new Toggle();
                    ToggleRight.RegisterValueChangedCallback((value) => { SetPortraitOrientation(PortraitOrientation.Right); });
                    ToggleRight.SetValueWithoutNotify(false);
                    ToggleRight.text = "Right";
                    ToggleRight.style.top = 1;
                    ToggleRight.style.marginLeft = 0;
                    ToggleRight.style.minWidth = 42;
                    decoratorFieldBottom.Add(ToggleRight);
                }
            }
        }

        var decoratorDivision = new VisualElement();
        decoratorDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        decoratorDivision.style.marginTop = 1;
        decoratorDivision.style.height = 1;
        Add(decoratorDivision);
    }

    public void SetPortraitType(PortraitType type)
    {
        PortraitType = type;
        EnumFieldPortrait.SetValueWithoutNotify(PortraitType);
    }
    public void SetEmotionType(EmotionType type)
    {
        EmotionType = type;
        EnumFieldEmotion.SetValueWithoutNotify(EmotionType);
    }
    public void SetPortraitOrientation(PortraitOrientation orientation)
    {
        PortraitOrientation = orientation;
        if (PortraitOrientation == PortraitOrientation.Left)
        {
            ToggleLeft.SetValueWithoutNotify(true);
            ToggleRight.SetValueWithoutNotify(false);
        }
        else
        {
            ToggleLeft.SetValueWithoutNotify(false);
            ToggleRight.SetValueWithoutNotify(true);
        }
    }
}
