using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class DialogueDecoratorPlaySFX : DialogueDecoratorBase
{
    public AudioClip clip;

    ObjectField AudioClipField;
    public DialogueDecoratorPlaySFX(DialogueNode node)
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

            var decoratorField = new VisualElement();
            decoratorField.style.flexDirection = FlexDirection.Column;
            decoratorBase.Add(decoratorField);
            // Decorator Field Top
            {
                var labelFrameCG = new Label("Play SFX");
                labelFrameCG.style.alignContent = Align.Center;
                labelFrameCG.style.marginLeft = 3;
                labelFrameCG.style.marginTop = 3;
                labelFrameCG.style.height = 16;
                decoratorField.Add(labelFrameCG);

                AudioClipField = new ObjectField()
                {
                    objectType = typeof(AudioClip),
                    allowSceneObjects = false,
                    value = clip
                };
                AudioClipField.SetValueWithoutNotify(clip);
                AudioClipField.RegisterValueChangedCallback((value) => { clip = (AudioClip)value.newValue; });
                AudioClipField.style.minWidth = 175;
                AudioClipField.style.maxWidth = 175;
                AudioClipField.style.marginLeft = 2;
                AudioClipField.style.marginRight = 0;
                decoratorField.Add(AudioClipField);
            }
        }

        var decoratorDivision = new VisualElement();
        decoratorDivision.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        decoratorDivision.style.marginTop = 1;
        decoratorDivision.style.height = 1;
        Add(decoratorDivision);
    }

    public void SetAudioClip(AudioClip audioClip)
    {
        clip = audioClip;
        AudioClipField.SetValueWithoutNotify(audioClip);
    }
}
