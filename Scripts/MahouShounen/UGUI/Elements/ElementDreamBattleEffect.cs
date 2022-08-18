using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable] public class OnEffectEndedEvent : UnityEvent<ElementDreamBattleEffect> { }

public class ElementDreamBattleEffect : MonoBehaviour
{
    public OnEffectEndedEvent onEffectEnded;

    Image image;
    RectTransform rectTransform;

    public void Awake()
    {
        image         = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetEffectData(ElementDreamBattleFigure elementFigure, SkillEffectData data)
    {
        BattleFigureInstanceType type = elementFigure.GetInstance().instanceType;
        RectTransform targetTransform = elementFigure.GetComponent<RectTransform>();

        Vector2 targetPosition = targetTransform.anchoredPosition;
        targetPosition += new Vector2(0f, (targetTransform.sizeDelta.y - 80f) / 2f);

        rectTransform.anchoredPosition = data.GetEffectPosition(targetPosition);
        rectTransform.sizeDelta        = data.size;

        if (type == BattleFigureInstanceType.Monster)
            rectTransform.localScale = new Vector3(-1, 1, 1);
        else
            rectTransform.localScale = Vector3.one;

        StartCoroutine(StartEffect(data));
    }

    IEnumerator StartEffect(SkillEffectData data)
    {
        int spriteIndex = -1;
        int spriteCount = data.sprites.Count;

        for (float i = 0; i < 1.0f; i += Time.deltaTime / data.timer)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = data.sprites[spriteIndex];
            }
            yield return null;
        }

        onEffectEnded.Invoke(this);
    }
}
