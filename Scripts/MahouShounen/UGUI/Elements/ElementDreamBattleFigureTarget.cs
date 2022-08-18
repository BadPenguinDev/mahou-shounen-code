using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleFigureTarget : MonoBehaviour
{
    [SerializeField] float moveTimer;
    [SerializeField] float idleTimer;
    [SerializeField] float idleY;

    RectTransform rectTransform;
    ElementDreamBattleFigure targetElement;

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartChangeTarget(ElementDreamBattleFigure figure)
    {
        if (targetElement == figure)
            return;

        targetElement = figure;

        RectTransform targetTransform = figure.GetComponent<RectTransform>();
        Vector2 targetPos = targetTransform.anchoredPosition + new Vector2(0f, targetTransform.sizeDelta.y);

        StopAllCoroutines();
        StartCoroutine(ChangeTarget(rectTransform.anchoredPosition, targetPos));
    }
    public void StartIdle()
    {
        StopAllCoroutines();
        StartCoroutine(Idle());
    }


    IEnumerator ChangeTarget(Vector2 begin, Vector2 end)
    {
        for (float i = 0; i < 1.0f; i += Time.deltaTime / moveTimer)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(begin, end, i * i);
            yield return null;
        }
        rectTransform.anchoredPosition = end;
        StartIdle();
    }
    IEnumerator Idle()
    {
        Vector2  startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = rectTransform.anchoredPosition + new Vector2(0f, idleY);

        while (true)
        {
            for (float i = 0; i < 1.0f; i += Time.deltaTime / idleTimer)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, i * i);
                yield return null;
            }
            rectTransform.anchoredPosition = targetPos;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / idleTimer)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetPos, startPos, i * i);
                yield return null;
            }
            rectTransform.anchoredPosition = startPos;
        }
    }
}
