using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class DialogueDecoratorFrameCG : DialogueDecoratorBase
{
    public Sprite SpriteFrameCG;

    ObjectField ImageField;
    public DialogueDecoratorFrameCG(DialogueNode node)
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
                Label labelFrameCG = new Label("Frame CG");
                labelFrameCG.style.alignContent = Align.Center;
                labelFrameCG.style.marginLeft = 3;
                labelFrameCG.style.marginTop = 3;
                labelFrameCG.style.height = 16;
                decoratorField.Add(labelFrameCG);

                ImageField = new ObjectField()
                {
                    objectType = typeof(Sprite),
                    allowSceneObjects = false,
                    value = SpriteFrameCG
                };
                ImageField.SetValueWithoutNotify(SpriteFrameCG);
                ImageField.RegisterValueChangedCallback((value) => { SpriteFrameCG = (Sprite)value.newValue; });
                ImageField.style.minWidth = 175;
                ImageField.style.maxWidth = 175;
                ImageField.style.marginLeft = 2;
                ImageField.style.marginRight = 0;
                decoratorField.Add(ImageField);
            }
        }

        var decoratorDivision = new VisualElement();
        decoratorDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        decoratorDivision.style.marginTop = 1;
        decoratorDivision.style.height = 1;
        Add(decoratorDivision);
    }

    public void SetSpriteFrameCG(Sprite sprite)
    {
        SpriteFrameCG = sprite;
        ImageField.SetValueWithoutNotify(sprite);
    }
}
