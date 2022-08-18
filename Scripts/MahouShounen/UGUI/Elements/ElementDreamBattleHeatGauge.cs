using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleHeatGauge : ElementBase
{
    [SerializeField] List<Sprite> heatGaugeOnSprites;
    [SerializeField] Vector2      heatGaugeOffModifier;
    [SerializeField] float        heatGaugeTimer;
    [SerializeField] int          heatGaugeWidth;

    Image image;
    RectTransform rectTransform;
    Vector3 originalPos;

    IEnumerator heatOnCoroutine;
    IEnumerator heatOffCoroutine;

    public int GetHeatGaugeWidth() { return heatGaugeWidth; }

    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        originalPos = rectTransform.anchoredPosition;
    }

    public void StartHeatOn()
    {
        if (heatOnCoroutine != null)
            StopCoroutine(heatOnCoroutine);

        heatOnCoroutine = HeatOn();
        StartCoroutine(heatOnCoroutine);
    }
    public void StartHeatOff()
    {
        if (heatOffCoroutine != null)
            StopCoroutine(heatOffCoroutine);

        heatOffCoroutine = HeatOff();
        StartCoroutine(heatOffCoroutine);
    }

    IEnumerator HeatOn()
    {
        Vector3 randomModifier = new Vector3();

        int spriteIndex = 0;
        int spriteCount = heatGaugeOnSprites.Count;

        for (float i = 0; i < 1.0f; i += Time.deltaTime / heatGaugeTimer)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = heatGaugeOnSprites[spriteIndex];

                randomModifier.x = Random.Range(-2.0f, 2.0f);
                randomModifier.y = Random.Range(-2.0f, 2.0f);

                rectTransform.anchoredPosition = originalPos + randomModifier;
            }
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
        yield return null;
    }
    IEnumerator HeatOff()
    {
        for (float i = 0; i < 1.0f; i += Time.deltaTime / heatGaugeTimer)
        {
            rectTransform.anchoredPosition = originalPos + ((Vector3)heatGaugeOffModifier * i);
            image.color = new Color(1f, 1f, 1f, 1f - i);

            yield return null;
        }

        image.sprite = heatGaugeOnSprites[0];
        image.color = new Color(1f, 1f, 1f, 1f);
        rectTransform.anchoredPosition = originalPos;

        yield return null;
    }
}
