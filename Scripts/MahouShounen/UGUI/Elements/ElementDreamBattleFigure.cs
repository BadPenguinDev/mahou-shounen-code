using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class OnFigureDeathEndedEvent : UnityEvent<ElementDreamBattleFigure> { }

public class ElementDreamBattleFigure : ElementBase
{
    [SerializeField] Button button;
    [SerializeField] ElementDreamBattleFigureStatus elementStatus;

    public OnFigureDeathEndedEvent onFigureDeathEnded;

    BattleFigureInstance figureInstance;
    BattleFigureData     targetFigure;

    bool isDead = false;

    IEnumerator spawnCoroutine;
    IEnumerator idleCoroutine;
    IEnumerator hitCoroutine;
    IEnumerator attackCoroutine;
    IEnumerator deathCoroutine;

    Image image;
    RectTransform rectTransform;
    RectTransform buttonTransform;


    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);

        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        buttonTransform = button.GetComponent<RectTransform>();

        elementStatus.SetParentComponent(this);
    }

    public Button               GetButton()   { return button; }
    public BattleFigureInstance GetInstance() { return figureInstance; }

    public void SetTargetFigure(BattleFigureInstance instance)
    {
        // Set Figure
        figureInstance = instance;
        targetFigure   = figureInstance.data;

        isDead = false;

        rectTransform.sizeDelta = targetFigure.spriteSize;

        buttonTransform.anchoredPosition = targetFigure.buttonPivot;
        buttonTransform.sizeDelta        = targetFigure.buttonSize;

        // Set Instance
        elementStatus.gameObject.SetActive(true);

        elementStatus.GetComponent<RectTransform>().anchoredPosition = targetFigure.buttonPivot;
        elementStatus.SetTargetFigure(figureInstance);

        // Set Instance - Event
        figureInstance.onFigureSpawn .AddListener(StartPlaySpawn);
        figureInstance.onFigureAttack.AddListener(StartPlayAttack);
        figureInstance.onFigureHit   .AddListener(StartPlayHit);
        figureInstance.onFigureDeath .AddListener(StartPlayDeath);

        StartPlayIdle();
    }

    public void StartPlaySpawn()
    {
        if (isDead)
            return;

        StopAllCoroutines();

        MinionFigureData minionFigure = targetFigure as MinionFigureData;
        spawnCoroutine = PlaySpawn(minionFigure.spawnTimer);
        StartCoroutine(spawnCoroutine);
    }
    public void StartPlayIdle()
    {
        if (isDead)
            return;

        StopAllCoroutines();

        idleCoroutine = PlayIdle(targetFigure.idleTimer);
        StartCoroutine(idleCoroutine);
    }
    public void StartPlayHit()
    {
        if (isDead)
            return;

        StopAllCoroutines();

        hitCoroutine = PlayHit(targetFigure.hitTimer);
        StartCoroutine(hitCoroutine);
    }
    public void StartPlayAttack()
    {
        if (isDead)
            return;

        StopAllCoroutines();

        attackCoroutine = PlayAttack(targetFigure.attackTimer);
        StartCoroutine(attackCoroutine);
    }
    public void StartPlayDeath()
    {
        isDead = true;

        StopAllCoroutines();

        deathCoroutine = PlayDeath(targetFigure.deathTimer);
        StartCoroutine(deathCoroutine);
    }

    IEnumerator PlaySpawn (float time)
    {
        MinionFigureData minionFigure = targetFigure as MinionFigureData;

        int spriteIndex = 0;
        int spriteCount = minionFigure.spawnSprite.Count;

        // Spawning Sequence
        image.sprite = minionFigure.spawnSprite[0];
        for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = minionFigure.spawnSprite[index];
            }
            yield return null;
        }

        StartPlayIdle();
    }
    IEnumerator PlayIdle  (float time)
    {
        int spriteIndex = 0;
        int spriteCount = targetFigure.idleSprite.Count;

        // Firing Sequence
        while (true)
        {
            spriteIndex = 0;
            image.sprite = targetFigure.idleSprite[0];

            for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
            {
                int index = (int)(i * spriteCount);
                if (spriteIndex < index)
                {
                    spriteIndex = index;
                    image.sprite = targetFigure.idleSprite[index];
                }
                yield return null;
            }
        }
    }
    IEnumerator PlayHit   (float time)
    {
        int spriteIndex = 0;
        int spriteCount = targetFigure.hitSprite.Count;

        // Firing Sequence
        image.sprite = targetFigure.hitSprite[0];
        for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = targetFigure.hitSprite[index];
            }
            yield return null;
        }

        StartPlayIdle();
    }
    IEnumerator PlayAttack(float time)
    {
        int spriteIndex = 0;
        int spriteCount = targetFigure.attackSprite.Count;

        // Firing Sequence
        image.sprite = targetFigure.attackSprite[0];
        for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = targetFigure.attackSprite[index];
            }
            yield return null;
        }

        StartPlayIdle();
    }
    IEnumerator PlayDeath (float time)
    {
        int spriteIndex = 0;
        int spriteCount = targetFigure.deathSprite.Count;

        // Firing Sequence
        image.sprite = targetFigure.hitSprite[0];
        for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
        {
            int index = (int)(i * spriteCount);
            if (spriteIndex < index)
            {
                spriteIndex = index;
                image.sprite = targetFigure.deathSprite[index];
            }
            yield return null;
        }

        // Element
        elementStatus.gameObject.SetActive(false);
        onFigureDeathEnded.Invoke(this);

        // Event 
        figureInstance.onFigureSpawn. RemoveListener(StartPlaySpawn); 
        figureInstance.onFigureAttack.RemoveListener(StartPlayAttack); 
        figureInstance.onFigureHit.   RemoveListener(StartPlayHit); 
        figureInstance.onFigureDeath. RemoveListener(StartPlayDeath); 
    }
}
