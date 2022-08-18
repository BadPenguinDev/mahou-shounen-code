using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class DialogueDecoratorTransition : DialogueDecoratorBase
{
    public GameObject Transition;

    ObjectField TransitionField;

    public DialogueDecoratorTransition(DialogueNode node)
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
            // Decorator Field Top
            {
                VisualElement decoratorFieldTop = new VisualElement();
                decoratorFieldTop.style.flexDirection = FlexDirection.Row;
                decoratorField.Add(decoratorFieldTop);
                // Decorator Field Top
                {
                    Label labelFrameCG = new Label("Transition");
                    labelFrameCG.style.alignContent = Align.Center;
                    labelFrameCG.style.marginLeft = 3;
                    labelFrameCG.style.marginTop = 3;
                    labelFrameCG.style.height = 16;
                    decoratorFieldTop.Add(labelFrameCG);
                }

                TransitionField = new ObjectField()
                {
                    objectType = typeof(GameObject),
                    allowSceneObjects = false,
                    value = Transition,
                };
                TransitionField.SetValueWithoutNotify(Transition);
                TransitionField.RegisterValueChangedCallback((value) => { Transition = (GameObject)value.newValue; });
                TransitionField.style.minWidth = 175;
                TransitionField.style.maxWidth = 175;
                TransitionField.style.marginLeft = 2;
                TransitionField.style.marginRight = 0;
                decoratorField.Add(TransitionField);
            }
        }

        var decoratorDivision = new VisualElement();
        decoratorDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        decoratorDivision.style.marginTop = 1;
        decoratorDivision.style.height = 1;
        Add(decoratorDivision);
    }

    public void SetAnimationCG(GameObject animationCG)
    {
        Transition = animationCG;
        TransitionField.SetValueWithoutNotify(Transition);
    }
}
