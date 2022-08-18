using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleBullet : ElementBase
{
    [SerializeField] List<Sprite> bulletSprites;
    [SerializeField] float reloadSlotModifierY;

    Image image;
    RectTransform rectTransform;
    Vector3 originalPos;

    bool isLoaded;

    public bool  IsLoaded()           { return isLoaded;            }
    public int   GetSpriteCount()     { return bulletSprites.Count; }

    IEnumerator reloadSlotCoroutine;

    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        originalPos = rectTransform.anchoredPosition;
    }

    public void FireBullet(int index)
    {
        image.sprite = bulletSprites[index];
    }
    public void SetLoaded(bool status)
    {
        isLoaded = status;
        if (isLoaded)
            image.sprite = bulletSprites[0];
        else
            image.sprite = bulletSprites[bulletSprites.Count - 1];
    }

    public void StartReloadSlot(float time, bool loaded)
    {
        if (reloadSlotCoroutine != null)
            StopCoroutine(reloadSlotCoroutine);

        reloadSlotCoroutine = ReloadSlot(time, loaded);
        StartCoroutine(reloadSlotCoroutine);
    }

    IEnumerator ReloadSlot(float time, bool loaded)
    {
        float rangeTime = time / 2.0f;

        for (float i = 0; i < 1.0f; i += Time.deltaTime / rangeTime)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i);
            rectTransform.anchoredPosition = originalPos + new Vector3(0f, i * reloadSlotModifierY, 0f);

            yield return null;
        }

        SetLoaded(loaded);

        for (float i = 1; i > 0.0f; i -= Time.deltaTime / rangeTime)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - i);
            rectTransform.anchoredPosition = originalPos + new Vector3(0f, i * reloadSlotModifierY, 0f);

            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
        yield return null;
    }
}
