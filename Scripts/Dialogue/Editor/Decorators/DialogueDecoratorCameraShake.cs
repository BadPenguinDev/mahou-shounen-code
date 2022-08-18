using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class DialogueDecoratorCameraShake : DialogueDecoratorBase
{
    public DialogueDecoratorCameraShake(DialogueNode node)
    {
        var decoratorBase = new VisualElement();
        decoratorBase.style.marginTop = 1;
        decoratorBase.style.flexDirection = FlexDirection.Row;
        Add(decoratorBase);
        // DecoratorBase
        {
            var buttonDelete = new Button(() => { node.DeleteDecorator(this); });
            buttonDelete.text = "X";
            buttonDelete.style.marginLeft = 3;
            buttonDelete.style.marginRight = 0;
            buttonDelete.style.marginTop = 1;
            buttonDelete.style.marginBottom = 1;
            decoratorBase.Add(buttonDelete);

            var labelFrameCG = new Label("Play Camera Shake");
            labelFrameCG.style.marginLeft = 3;
            labelFrameCG.style.marginTop = 2;
            labelFrameCG.style.height = 16;
            labelFrameCG.style.top = 1;
            labelFrameCG.style.minWidth = 60;
            decoratorBase.Add(labelFrameCG);
        }

        var decoratorDivision = new VisualElement();
        decoratorDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        decoratorDivision.style.marginTop = 1;
        decoratorDivision.style.height = 1;
        Add(decoratorDivision);
    }
}
