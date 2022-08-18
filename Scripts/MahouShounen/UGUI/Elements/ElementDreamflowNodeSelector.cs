using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementDreamflowNodeSelector : MonoBehaviour
{
    [SerializeField] float moveTimer;
    [SerializeField] float idleTimer;

    RectTransform rectTransform;
    IEnumerator currentCoroutine;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartMove(RectTransform elementTransform)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = Move(elementTransform);
        StartCoroutine(currentCoroutine);
    }
    public void StartIdle()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = Idle();
        StartCoroutine(currentCoroutine);
    }

    IEnumerator Move(RectTransform elementTransform)
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2  targetPos = elementTransform.anchoredPosition;
        
        for (float t = 0; t < 1.0f; t += Time.deltaTime / moveTimer)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, targetPos, t * t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
        StartIdle();
    }
    IEnumerator Idle()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2  targetPos = currentPos - new Vector2(0f, 4f);

        while (true)
        {
            for (float t = 0; t < 1.0f; t += Time.deltaTime / (idleTimer  / 2f))
            {
                rectTransform.anchoredPosition = Vector2.Lerp(currentPos, targetPos, t * t);
                yield return null;
            }

            for (float t = 0; t < 1.0f; t += Time.deltaTime / (idleTimer / 2f))
            {
                rectTransform.anchoredPosition = Vector2.Lerp(targetPos, currentPos, t * t);
                yield return null;
            }
        }
    }
}
