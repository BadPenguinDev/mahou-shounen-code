using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Localization.Components;


[System.Flags]
public enum DreamBattleAnimState { Firing = 1 << 0, Spinning = 1 << 1, Reloading = 1 << 2 }

[System.Serializable] 
public class OnTargetInstanceChangedEvent : UnityEvent<BattleFigureInstance> { }

public class LayoutDreamBattle : LayoutBase
{
    [Header("Battle Figures")] 
    [SerializeField]      ElementDreamBattleFigure   playerFigure; 
    [SerializeField] List<ElementDreamBattleFigure> monsterFigures; 
    [SerializeField] List<ElementDreamBattleFigure>  minionFigures; 

    [Header("Battle Figure Target")] 
    [SerializeField] ElementDreamBattleFigureTarget elementFigureTarget; 
    [SerializeField] ElementDreamBattleFigure       targetFigure;

    OnTargetInstanceChangedEvent onTargetInstanceChanged;

    [Header("Skill Effects")]
    [SerializeField] GameObject prefabElementEffect;
    [SerializeField] Transform  panelEffectElement;

    List<ElementDreamBattleEffect>   activeEffectElements;
    List<ElementDreamBattleEffect> inactiveEffectElements;

    [Header("Battle Results")]
    [SerializeField] float battleResultTimer;
    [SerializeField] Image imageBattleResult;
    [SerializeField] List<Sprite> spriteResults;


    [Header("Cylinder")] 
    [SerializeField] ElementDreamBattleCylinder elementCylinder;

    [Header("Skill Slots")] 
    [SerializeField] RectTransform panelSkillSlot; 
    [SerializeField] GameObject prefabSkillSlot; 
    [SerializeField] Vector2 elementSkillSlotPivot; 
    [SerializeField] Vector2 elementSkillSlotSize;

    int currentSkillSlotCount;
    int     maxSkillSlotCount;

    List<ElementDreamBattleSkillSlot> elementSkillSlots;

    [Header("Skill Slots - Coroutine")]
    [SerializeField] Image imageSkillSlotFireEffect;

    [SerializeField] float   fireSkillSlotTimer;
    [SerializeField] float   spinSkillSlotTimer;
    [SerializeField] float reloadSkillSlotTimer;

    IEnumerator   fireSkillSlotCoroutine;
    IEnumerator   spinSkillSlotCoroutine;
    IEnumerator reloadSkillSlotCoroutine;

    [Header("Heat Gauge")]
    [SerializeField] RectTransform boxHeatGauges;
    [SerializeField] GameObject prefabHeatGauge;
    List<ElementDreamBattleHeatGauge> elementHeatGauges;

    [SerializeField] Vector2 boxHeatGaugePivot;
    [SerializeField] Vector2 boxHeatGaugeSize;

    int currentHeatGauge = 0;
    int     maxHeatGauge = 0;

    [SerializeField] DreamBattleAnimState layoutDreamBattleAnimState;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        var battleController = UGUIManager.Get().GetComponent<MSBattleController>();
        battleController.onBattleVictory.AddListener(OnBattleVictory);
        battleController.onBattleDefeat.AddListener(OnBattleDefeat);

        #region Battle Figures
        playerFigure.SetParentComponent(this);
        playerFigure.onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);

        // Set Data and Bind - Monsters
        for (var i = 0; i < monsterFigures.Count; i++)
        {
            monsterFigures[i].SetParentComponent(this);
            monsterFigures[i].onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);
        }

        // Set Data and Bind - Summons
        battleController.onMinionSummoned.AddListener(OnMinionSummoned);

        for (var i = 0; i < minionFigures.Count; i++)
        {
            minionFigures[i].SetParentComponent(this);
            minionFigures[i].onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);
        }
        #endregion

        #region Battle Figure Target 
        onTargetInstanceChanged = new OnTargetInstanceChangedEvent();
        onTargetInstanceChanged.AddListener(battleController.SetTargetFigure);
        #endregion

        #region Battle Figure Effect 
          activeEffectElements = new List<ElementDreamBattleEffect>();
        inactiveEffectElements = new List<ElementDreamBattleEffect>();
        #endregion

        #region Cylinder 
        elementCylinder.SetParentComponent(this);

        battleController.onFire.  AddListener(elementCylinder.StartFire);
        battleController.onSpin.  AddListener(elementCylinder.StartSpin);
        battleController.onReload.AddListener(elementCylinder.StartReload);

        elementCylinder.onFireGUI.  AddListener(battleController.Fire);
        elementCylinder.onSpinGUI.  AddListener(battleController.Spin);
        elementCylinder.onReloadGUI.AddListener(battleController.Reload);
        #endregion
        
        #region Skill Slots 
        currentSkillSlotCount = 0;
            maxSkillSlotCount = MSGameInstance.Get().maxCylinderSlots;

        if (elementSkillSlots == null)
        {
            elementSkillSlots = new List<ElementDreamBattleSkillSlot>();
            for (var i = 0; i < 6; i++)
            {
                var skillSlotInstance = Instantiate(prefabSkillSlot, panelSkillSlot);

                var element = skillSlotInstance.GetComponent<ElementDreamBattleSkillSlot>();
                element.SetParentComponent(this);

                elementSkillSlots.Add(element);
            }
        }

        battleController.onFire.  AddListener(StartFireSkillSlot);
        battleController.onSpin.  AddListener(StartSpinSkillSlot);
        battleController.onReload.AddListener(StartReloadSkillSlot);
        #endregion

        #region Heat Gauge 
        elementHeatGauges = new List<ElementDreamBattleHeatGauge>();
        battleController.onHeatGaugeChanged.AddListener(SetHeatGaugeCount);
        #endregion
    }
    public override void OpenLayout(string layoutName)
    {
        base.OpenLayout(layoutName);

        OnBattleStart();
    }

    public void OnBattleStart()
    {
        var battleController = UGUIManager.Get().GetComponent<MSBattleController>();
        imageBattleResult.gameObject.SetActive(false);

        #region Battle Figures
             BattleFigureInstance   playerInstance;
        List<BattleFigureInstance> monsterInstances;
        List<BattleFigureInstance>  minionInstances;

        battleController.GetFigureInstances(out playerInstance, out monsterInstances, out minionInstances);

        // Set Data and Bind - Player
        playerInstance.onFigureEffectAdded.AddListener(SpawnFigureEffect);

        playerFigure.SetTargetFigure(playerInstance);
        playerFigure.onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);

        // Set Data and Bind - Monsters
        for (var i = 0; i < monsterInstances.Count; i++)
        {
            if (monsterInstances[i] == null)
            {
                monsterFigures[i].gameObject.SetActive(false);
                continue;
            }

            monsterInstances[i].onFigureEffectAdded.AddListener(SpawnFigureEffect);

            monsterFigures[i].gameObject.SetActive(true);
            monsterFigures[i].SetTargetFigure(monsterInstances[i]);
            monsterFigures[i].onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);
        }

        // Set Data and Bind - Summons
        battleController.onMinionSummoned.AddListener(OnMinionSummoned);

        for (var i = 0; i < minionInstances.Count; i++)
        {
            if (minionInstances[i] == null)
            {
                minionFigures[i].gameObject.SetActive(false);
                continue;
            }

            minionInstances[i].onFigureEffectAdded.AddListener(SpawnFigureEffect);

            minionFigures[i].gameObject.SetActive(true);
            minionFigures[i].SetTargetFigure(minionInstances[i]);
            minionFigures[i].onFigureDeathEnded.AddListener(OnTargetFigureDeathEnded);
        }

        SetTargetFigure(monsterFigures[0]);
        #endregion

        #region Skill Slots 
        currentSkillSlotCount = battleController.GetLoadedSkillCount();
            maxSkillSlotCount = MSGameInstance.Get().maxCylinderSlots;

        for (var i = 0; i < 6; i++)
        {
            var rectTransform = elementSkillSlots[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = elementSkillSlotPivot;

            elementSkillSlots[i].SetParentComponent(this);
            elementSkillSlots[i].ClearSkillData();
            rectTransform.anchoredPosition += new Vector2(i * elementSkillSlotSize.x, 0f);
        }
        #endregion


        #region Heat Gauge 
        boxHeatGauges.sizeDelta = boxHeatGaugeSize;
        maxHeatGauge = MSGameInstance.Get().maxHeatGauge;

        for (var i = 0; i < maxHeatGauge; i++)
        {
            ElementDreamBattleHeatGauge element;

            if (i >= elementHeatGauges.Count)
            {
                var heatGaugeInstance = Instantiate(prefabHeatGauge, boxHeatGauges);

                var rectTransform = heatGaugeInstance.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = boxHeatGaugePivot;

                element = heatGaugeInstance.GetComponent<ElementDreamBattleHeatGauge>();
                element.SetParentComponent(this);

                rectTransform.anchoredPosition += new Vector2(i * element.GetHeatGaugeWidth(), 0f);

                elementHeatGauges.Add(element);
            }
            else
                element = elementHeatGauges[i];

            boxHeatGauges.sizeDelta += new Vector2(element.GetHeatGaugeWidth(), 0f);
        }
        #endregion


        battleController.Reload();
    }
    public void OnBattleVictory()
    {
        imageBattleResult.gameObject.SetActive(true);
        imageBattleResult.sprite = spriteResults[0];

        StartCoroutine(DelayBattleResult(true));
    }
    public void OnBattleDefeat()
    {
        imageBattleResult.gameObject.SetActive(true);
        imageBattleResult.sprite = spriteResults[1];

        StartCoroutine(DelayBattleResult(false));
    }

    IEnumerator DelayBattleResult(bool victory)
    {
        yield return new WaitForSeconds(battleResultTimer);
        if (victory)
        {
            manager.OpenLayout(LayoutType.Dreamflow);
            CloseLayout();

            var layoutDreamflow = manager.GetLayout(LayoutType.Dreamflow) as LayoutDreamflow;
            layoutDreamflow.SetBattleEnded();
        }
        else
        {
            CloseLayout();
            if (MSEventManager.Get().PlayDreampostDefeatEvent()) 
                yield break;
            
            manager.OpenLayout(LayoutType.Main);
            manager.SetLayoutMainMode(LayoutMainMode.Schedule);
        }
    }

    // Figure 
    public void OnTargetFigureDeathEnded(ElementDreamBattleFigure element)
    {
        element.GetInstance().onFigureEffectAdded.RemoveListener(SpawnFigureEffect);

        if (targetFigure != element)
            return;

        for (int i = 0; i < monsterFigures.Count; i++)
        {
            BattleFigureInstance instance = monsterFigures[i].GetInstance();
            if (instance != null)
            {
                if (instance.hp == 0)
                    continue;

                SetTargetFigure(monsterFigures[i]);
                break;
            }
        }
    }
    public void SetTargetFigure(ElementDreamBattleFigure element)
    {
        if (element.GetInstance() == null)
            return;

        if (element.GetInstance().hp == 0)
            return;

        if (element.GetInstance().instanceType != BattleFigureInstanceType.Monster)
            return;

        targetFigure = element;
        elementFigureTarget.StartChangeTarget(targetFigure);

        onTargetInstanceChanged.Invoke(element.GetInstance());
    }
    public void OnMinionSummoned(BattleFigureInstance instance)
    {
        MinionFigureData data = instance.data as MinionFigureData;
        if (data.formation == MinionFormationType.Front)
        {
            minionFigures[0].gameObject.SetActive(true);
            minionFigures[0].SetTargetFigure(instance);
        }
        else
        {
            minionFigures[1].gameObject.SetActive(true);
            minionFigures[1].SetTargetFigure(instance);
        }

        instance.onFigureEffectAdded.AddListener(SpawnFigureEffect);
    }

    // Effect 
    public void SpawnFigureEffect(BattleFigureInstance instance, SkillEffectData data)
    {
        ElementDreamBattleEffect element = null;
        if (inactiveEffectElements.Count > 0)
        {
            element = inactiveEffectElements[0];
            element.gameObject.SetActive(true);
        }
        else
        {
            GameObject prefab = Instantiate(prefabElementEffect, panelEffectElement);
            element = prefab.GetComponent<ElementDreamBattleEffect>();
            element.onEffectEnded.AddListener(OnFigureEffectEnded);
        }

        if (instance.instanceType == BattleFigureInstanceType.Player)
        {
            element.SetEffectData(playerFigure, data);
        }
        else if (instance.instanceType == BattleFigureInstanceType.Monster)
        {
            for (int i = 0; i < monsterFigures.Count; i++)
            {
                if (monsterFigures[i].GetInstance() == instance)
                    element.SetEffectData(monsterFigures[i], data);
            }
        }
        else
        {
            for (int i = 0; i < minionFigures.Count; i++)
            {
                if (minionFigures[i].GetInstance() == instance)
                    element.SetEffectData(minionFigures[i], data);
            }
        }

        activeEffectElements.Add(element);

        if (inactiveEffectElements.Contains(element))
            inactiveEffectElements.Remove(element);
    }
    public void OnFigureEffectEnded(ElementDreamBattleEffect element)
    {
        if (!activeEffectElements.Contains(element)) 
            return;
        
        element.gameObject.SetActive(false);

        activeEffectElements.Remove(element);
        inactiveEffectElements.Add(element);
    }

    // Cylinder 
    public void StartFireSkillSlot()
    {
        if (fireSkillSlotCoroutine != null)
            StopCoroutine(fireSkillSlotCoroutine);

        fireSkillSlotCoroutine = FireSkillSlot();
        StartCoroutine(fireSkillSlotCoroutine);
    }
    public void StartSpinSkillSlot()
    {
        if (spinSkillSlotCoroutine != null)
            StopCoroutine(spinSkillSlotCoroutine);

        spinSkillSlotCoroutine = SpinSkillSlot();
        StartCoroutine(spinSkillSlotCoroutine);
    }
    public void StartReloadSkillSlot(List<Skill> skills)
    {
        if (reloadSkillSlotCoroutine != null)
            StopCoroutine(reloadSkillSlotCoroutine);

        reloadSkillSlotCoroutine = ReloadSkillSlot(skills);
        StartCoroutine(reloadSkillSlotCoroutine);
    }

    public void SetHeatGaugeCount(int count)
    {
        if (currentHeatGauge < count)
        {
            for (int i = currentHeatGauge; i < count; i++)
            {
                elementHeatGauges[i].StartHeatOn();
            }
        }
        else
        {
            for (int i = currentHeatGauge - 1; i >= count; i--)
            {
                elementHeatGauges[i].StartHeatOff();
            }
        }
        currentHeatGauge = count;

        elementCylinder.SetCylinderHeat(currentHeatGauge, maxHeatGauge);
    }

    public bool IsLayoutDreamBattleAnimPlaying()
    {
        return (layoutDreamBattleAnimState & DreamBattleAnimState.Firing)    != 0 ||
               (layoutDreamBattleAnimState & DreamBattleAnimState.Spinning)  != 0 ||
               (layoutDreamBattleAnimState & DreamBattleAnimState.Reloading) != 0;
    }

    // Coroutine
    IEnumerator FireSkillSlot()
    {
        layoutDreamBattleAnimState |= DreamBattleAnimState.Firing;

        currentSkillSlotCount -= 1;

        var effectAlpha = new Color(1f, 1f, 1f, 1f);
        var randomModifier = new Vector3();

        var   slotTransform =     elementSkillSlots[0].GetComponent<RectTransform>();
        var effectTransform = imageSkillSlotFireEffect.GetComponent<RectTransform>();

        Vector3   slotPosition =   slotTransform.anchoredPosition;
        Vector3 effectPosition = effectTransform.anchoredPosition;

        // Flash On
        for (float i = 0; i < 1.0f; i += Time.deltaTime / fireSkillSlotTimer / 0.2f)
        {
            effectAlpha.a = i;
            imageSkillSlotFireEffect.color = effectAlpha;

            randomModifier.x = Random.Range(-2.0f, 2.0f);
            randomModifier.y = Random.Range(-2.0f, 2.0f);

              slotTransform.anchoredPosition =   slotPosition + randomModifier;
            effectTransform.anchoredPosition = effectPosition + randomModifier;

            yield return null;
        }

        // Empty Slot
        elementSkillSlots[0].ClearSkillData();
        slotTransform.anchoredPosition = slotPosition;

        // Flash Off
        for (float i = 0; i < 1.0f; i += Time.deltaTime / fireSkillSlotTimer / 0.8f)
        {
            effectAlpha.a = 1 - i;
            imageSkillSlotFireEffect.color = effectAlpha;

            randomModifier.x = Random.Range(-2.0f, 2.0f);
            randomModifier.y = Random.Range(-2.0f, 2.0f);

            effectTransform.anchoredPosition = effectPosition + randomModifier;

            yield return null;
        }
        effectTransform.anchoredPosition = effectPosition;

        // Check Empty
        var isAllSlotEmpty = true;
        for (var i = 0; i < 6; i++)
        {
            if (elementSkillSlots[i].isSlotEmpty) 
                continue;
            
            isAllSlotEmpty = false;
            break;
        }

        if (!isAllSlotEmpty)
            StartSpinSkillSlot();

        layoutDreamBattleAnimState &= ~DreamBattleAnimState.Firing;
        yield return null;
    }
    IEnumerator SpinSkillSlot()
    {
        layoutDreamBattleAnimState |= DreamBattleAnimState.Spinning;

        // Init
        var slotWidthVector = new Vector2(elementSkillSlotSize.x, 0f);

        var elementTransforms  = new List<RectTransform>();
        var elementOriginalPos = new List<Vector2>();

        for (var i = 0; i < 6; i++)
        {
            elementTransforms. Add(elementSkillSlots[i].GetComponent<RectTransform>());
            elementOriginalPos.Add(elementTransforms[i].anchoredPosition);
        }

        // Spin
        for (float i = 0; i < 1.0f; i += Time.deltaTime / spinSkillSlotTimer / 0.4f)
        {
            for (var j = 0; j < 6; j++)
            {
                elementTransforms[j].anchoredPosition = elementOriginalPos[j]
                    - (i * slotWidthVector * 1.5f);
            }
            yield return null;
        }

        // Slot Change
        var element = elementSkillSlots[0];
        var elementTransform = elementTransforms[0];

        var isFired = elementSkillSlots[0].isSlotEmpty;
        var tempSkillSlotCount = currentSkillSlotCount;
        if (isFired)
            tempSkillSlotCount++;

        for (var i = 1; i < tempSkillSlotCount; i++)
        {
            elementSkillSlots[i - 1] = elementSkillSlots[i];
            elementTransforms[i - 1] = elementTransforms[i];

            if (i == tempSkillSlotCount - 1)
            {
                elementSkillSlots[i] = element;
                elementTransforms[i] = elementTransform;
            }
        }

        // Spin - Recoil
        for (float i = 0; i < 1.0f; i += Time.deltaTime / spinSkillSlotTimer / 0.1f)
        {
            for (var j = 0; j < 6; j++)
            {
                if (!isFired && 
                    j == currentSkillSlotCount - 1)
                    continue;

                elementTransforms[j].anchoredPosition = elementOriginalPos[j]
                    - ((1.0f - i) * slotWidthVector * 0.5f);
            }
            yield return null;
        }

        // Set Original Pos
        for (var i = 0; i < 6; i++)
        {
            elementTransforms[i].anchoredPosition = elementOriginalPos[i];
        }

        // Load Slot - Recoil
        var lastIndex = currentSkillSlotCount - 1;
        if (!isFired)
        {
            for (float i = 0; i < 1.0f; i += Time.deltaTime / spinSkillSlotTimer / 0.4f)
            {
                elementTransforms[lastIndex].anchoredPosition = elementOriginalPos[lastIndex]
                    + ((1.0f - i) * slotWidthVector * (6.5f - lastIndex))
                    - (slotWidthVector * 0.5f);

                yield return null;
            }

            for (float i = 0; i < 1.0f; i += Time.deltaTime / spinSkillSlotTimer / 0.1f)
            {
                elementTransforms[lastIndex].anchoredPosition = elementOriginalPos[lastIndex]
                    - ((1.0f - i) * slotWidthVector * 0.5f);

                yield return null;
            }
        }

        // Set Original Pos
        elementTransforms[lastIndex].anchoredPosition = elementOriginalPos[lastIndex];

        layoutDreamBattleAnimState &= ~DreamBattleAnimState.Spinning;
        yield return null;
    }
    IEnumerator ReloadSkillSlot(List<Skill> skills)
    {
        layoutDreamBattleAnimState |= DreamBattleAnimState.Reloading;

        currentSkillSlotCount = skills.Count;
        var slotWidthVector = new Vector2(elementSkillSlotSize.x, 0f);

        // Init
        var elementTransforms  = new List<RectTransform>();
        var elementOriginalPos = new List<Vector2>(); 
        for (var i = 0; i < 6; i++)
        {
            elementTransforms. Add(elementSkillSlots[i].GetComponent<RectTransform>());
            elementOriginalPos.Add(elementTransforms[i].anchoredPosition);
        }

        // Empty Slot
        for (float i = 0; i < 1.0f; i += Time.deltaTime / reloadSkillSlotTimer / 0.5f)
        {
            for (var j = 0; j < currentSkillSlotCount; j++)
            {
                elementTransforms[j].anchoredPosition = elementOriginalPos[j]
                    - (i * (slotWidthVector * (j + 6)));
            }
            yield return null;
        }

        // Set SkillData 
        for (var i = 0; i < skills.Count; i++)
        {
            elementSkillSlots[i].SetSkillData(skills[i]);
        }

        // Load Slot
        for (float i = 0; i < 1.0f; i += Time.deltaTime / reloadSkillSlotTimer / 0.4f)
        {
            for (var j = 0; j < 6; j++)
            {
                elementTransforms[j].anchoredPosition = elementOriginalPos[j]
                    + ((1.0f - i) * (slotWidthVector * (j + 6f)))
                    - (slotWidthVector / 2f);
            }
            yield return null;
        }

        // Load Slot - Recoil
        for (float i = 0; i < 1.0f; i += Time.deltaTime / reloadSkillSlotTimer / 0.1f)
        {
            for (var j = 0; j < 6; j++)
            {
                elementTransforms[j].anchoredPosition = elementOriginalPos[j]
                    - ((1.0f - i) * slotWidthVector * (0.5f));
            }
            yield return null;
        }

        // Set Original Pos
        for (var i = 0; i < 6; i++)
        {
            elementTransforms[i].anchoredPosition = elementOriginalPos[i];
        }

        layoutDreamBattleAnimState &= ~DreamBattleAnimState.Reloading;
        yield return null;
    }
}
