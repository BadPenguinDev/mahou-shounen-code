using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ElementDreamBattleCylinder : ElementBase
{
    [SerializeField] List<ElementDreamBattleBullet> bulletComponents;

    [SerializeField] Image imageCylinderHeat;
    [SerializeField] float heatEffectStartPercent;

    [SerializeField] float   fireTimer;
    [SerializeField] float   spinTimer;
    [SerializeField] float reloadTimer;
    
    [SerializeField] AudioClip clipFire;
    [SerializeField] AudioClip clipSpin;
    [SerializeField] AudioClip clipReload;


    public UnityEvent onFireGUI;
    public UnityEvent onSpinGUI;
    public UnityEvent onReloadGUI;


    LayoutDreamBattle parentLayout;

    RectTransform rectTransform;
    EventTrigger  eventTrigger;

    bool isCylinderDragging;
    Vector2 mousePosition;

    Vector3 originalPos;

    DreamBattleAnimState cylinderAnimState;

    IEnumerator   fireCoroutine;
    IEnumerator   spinCoroutine;
    IEnumerator reloadCoroutine;



    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
        parentLayout = component as LayoutDreamBattle;

        rectTransform = GetComponent<RectTransform>();
        eventTrigger  = GetComponent<EventTrigger>();

        originalPos = rectTransform.anchoredPosition;

        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDragCylinder((PointerEventData)data); });
        eventTrigger.triggers.Add(beginDragEntry);

        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDragCylinder((PointerEventData)data); });
        eventTrigger.triggers.Add(endDragEntry);
    }

    private void OnBeginDragCylinder(PointerEventData data)
    {
        isCylinderDragging = true;
        mousePosition = data.position;
    }
    private void OnEndDragCylinder(PointerEventData data)
    {
        if (isCylinderDragging == false)
            return;

        mousePosition -= data.position;
        if (mousePosition.x < -28)
            ReloadGUI();
        else if (mousePosition.x > 28)
            SpinGUI();
        else
            FireGUI();
    }


    bool IsCylinderAnimPlaying()
    {
        if ((cylinderAnimState & DreamBattleAnimState.Firing)    != 0 ||
            (cylinderAnimState & DreamBattleAnimState.Spinning)  != 0 ||
            (cylinderAnimState & DreamBattleAnimState.Reloading) != 0)
            return true;

        return false;
    }

    public void FireGUI()
    {
        if (IsCylinderAnimPlaying() || 
            parentLayout.IsLayoutDreamBattleAnimPlaying())
            return;

        onFireGUI.Invoke();
    }
    public void SpinGUI()
    {
        if (IsCylinderAnimPlaying() ||
            parentLayout.IsLayoutDreamBattleAnimPlaying())
            return;

        onSpinGUI.Invoke();
    }
    public void ReloadGUI()
    {
        if (IsCylinderAnimPlaying() ||
            parentLayout.IsLayoutDreamBattleAnimPlaying())
            return;

        onReloadGUI.Invoke();
    }

    public void StartFire()
    {
        if (fireCoroutine != null)
            StopCoroutine(fireCoroutine);

        fireCoroutine = Fire(fireTimer);
        StartCoroutine(fireCoroutine);
    }
    public void StartSpin()
    {
        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);

        spinCoroutine = Spin(spinTimer);
        StartCoroutine(spinCoroutine);
    }
    public void StartReload(List<Skill> skills)
    {
        if (reloadCoroutine != null)
            StopAllCoroutines();

        reloadCoroutine = Reload(reloadTimer, skills.Count);
        StartCoroutine(reloadCoroutine);
    }

    public void SetCylinderHeat(int currentHeatGauge, int maxHeatGauge)
    {
        var heatEffectStartCount = (int)(maxHeatGauge * heatEffectStartPercent);

        var alpha = (currentHeatGauge - heatEffectStartCount) / 
                 (float)(    maxHeatGauge - heatEffectStartCount);
        if (alpha < 0)
            alpha = 0;

        imageCylinderHeat.color = new Color(1f, 1f, 1f, alpha);
    }

    IEnumerator Fire(float time)
    {
        var randomModifier = new Vector3();
        
        // Check Bullet Loaded
        if (bulletComponents[0].IsLoaded())
        {
            cylinderAnimState |= DreamBattleAnimState.Firing;

            manager.StopPlayAudioToggle();
            manager.PlayAudioClip(clipFire);

            var spriteIndex = 0;
            var spriteCount = bulletComponents[0].GetSpriteCount();

            // Firing Sequence
            for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
            {
                var index = (int)(i * spriteCount);
                if (spriteIndex < index)
                {
                    spriteIndex = index;
                    bulletComponents[0].FireBullet(index);

                    randomModifier.x = Random.Range(-2.0f, 2.0f);
                    randomModifier.y = Random.Range(-2.0f, 2.0f);

                    rectTransform.anchoredPosition = originalPos + randomModifier;
                }
                yield return null;
            }

            // End Fire
            bulletComponents[0].SetLoaded(false);
            rectTransform.anchoredPosition = originalPos;

            cylinderAnimState &= ~DreamBattleAnimState.Firing;
            rectTransform.eulerAngles = new Vector3(0f, 0f, 0f);

            StartSpin();
        }

        yield return null;
    }
    IEnumerator Spin(float time)
    {
        if (!IsAllBulletUnloaded())
        {
            cylinderAnimState |= DreamBattleAnimState.Spinning;

            var spinnnigCount = GetUnloadedBulletGap();
            for (var i = 0; i< spinnnigCount; i++)
            {
                manager.StopPlayAudioToggle();
                manager.PlayAudioClip(clipSpin);
                
                // Spinning
                for (float j = 0; j < 1.0f; j += Time.deltaTime / time)
                {
                    rectTransform.eulerAngles = new Vector3(0f, 0f, j * 70.0f);
                    yield return null;
                }
                rectTransform.eulerAngles = new Vector3(0f, 0f, 10f);

                // Changing
                var lastBulletLoaded = bulletComponents[0].IsLoaded();
                for (var j = 1; j < bulletComponents.Count; j++)
                {
                    var loaded = bulletComponents[j].IsLoaded();
                    bulletComponents[j - 1].SetLoaded(loaded);
                }

                var lastIndex = bulletComponents.Count - 1;
                bulletComponents[lastIndex].SetLoaded(lastBulletLoaded);

                // Cocking
                for (var j = 0.0f; j < 1.0f; j += Time.deltaTime / time)
                {
                    var zAngle = 10f - (j * 10.0f);
                    rectTransform.eulerAngles = new Vector3(0f, 0f, zAngle);
                    yield return null;
                }
            }
            cylinderAnimState &= ~DreamBattleAnimState.Spinning;
        }
        rectTransform.eulerAngles = new Vector3(0f, 0f, 0f);

        yield return null;
    }
    IEnumerator Reload(float time, int count)
    {
        cylinderAnimState |= DreamBattleAnimState.Reloading;

        manager.StopPlayAudioToggle();
        manager.PlayAudioClip(clipReload);
        
        for (var i = 0.0f; i < 1.0f; i += Time.deltaTime / 0.05f)
        {
            rectTransform.eulerAngles = new Vector3(0f, 0f, i * -30.0f);
            yield return null;
        }

        for (var i = 1.0f; i > 0; i -= Time.deltaTime / 0.05f)
        {
            rectTransform.eulerAngles = new Vector3(0f, 0f, i * -30.0f);
            yield return null;
        }

        for (var i = 0; i < bulletComponents.Count ; i++)
        {
            bulletComponents[i].StartReloadSlot(time, i < count);
        }

        for (float i = 0; i < 1.0f; i += Time.deltaTime / time)
        {
            rectTransform.eulerAngles = new Vector3(0f, 0f, i * 360.0f);
            yield return null;
        }

        cylinderAnimState &= ~DreamBattleAnimState.Reloading;
        rectTransform.eulerAngles = new Vector3(0f, 0f, 0f);
        yield return null;
    }

    bool IsAllBulletUnloaded()
    {
        foreach (var element in bulletComponents)
        {
            if (element.IsLoaded())
                return false;
        }

        return true;
    }
    int GetUnloadedBulletGap()
    {
        int gap;
        for (gap = 1; gap < bulletComponents.Count; gap++)
        {
            if (bulletComponents[gap].IsLoaded())
                break;
        }
        return gap;
    }
}
