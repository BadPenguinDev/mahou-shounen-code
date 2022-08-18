using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class OnBattleVictoryEvent : UnityEvent { }
public class OnBattleDefeatEvent  : UnityEvent { }

// Timer
public class OnBattleTickEndedEvent  : UnityEvent { }
public class OnHeatGaugeChangedEvent : UnityEvent<int> { }

// Figure
public class OnMinionSummonedEvent  : UnityEvent<BattleFigureInstance> { }
public class OnSendFigureDeathEvent : UnityEvent<BattleFigureInstance> { }

// Player
public class OnFireEvent   : UnityEvent { }
public class OnSpinEvent   : UnityEvent { }
public class OnReloadEvent : UnityEvent<List<Skill>> { }

public class OnOverheatStartedEvent  : UnityEvent { }
public class OnOverheatEndedEvent    : UnityEvent { }

public enum BattleFigureInstanceType { Player, Monster, Minion }


public class MSBattleController : MonoBehaviour
{
    [SerializeField] PlayerFigureData playerFigure;
    [SerializeField] DreamflowNodeData node;

    [SerializeField] SkillEffectData defenseSkillEffect;

    [SerializeField] float battleTickTimer;
    [SerializeField] float multipleHitTimer;
    [SerializeField] float battleResultTimer;

    [SerializeField] public EventData defaultDefeatEvent;


    int heatGauge;
    bool isOverheated;
    bool isBattleTickBegin;

    List<Skill>   loadedSkillQueue;
    List<Skill> unloadedSkillQueue;


    // Battle Staus
    public OnBattleVictoryEvent onBattleVictory;
    public OnBattleDefeatEvent  onBattleDefeat;

    // Timer
    public OnBattleTickEndedEvent  onBattleTickEnded;
    public OnHeatGaugeChangedEvent onHeatGaugeChanged;

    // Figure
    public OnMinionSummonedEvent  onMinionSummoned;

    // Player
    public OnFireEvent      onFire;
    public OnSpinEvent      onSpin;
    public OnReloadEvent    onReload;

    public OnOverheatStartedEvent onOverheatStarted;
    public OnOverheatEndedEvent   onOverheatEnded;

    // Instance
         BattleFigureInstance   playerInstance;
    List<BattleFigureInstance> monsterInstances;
    List<BattleFigureInstance>  minionInstances;

    BattleFigureInstance targetInstance;


    public void Awake()
    {
        onBattleVictory = new OnBattleVictoryEvent();
        onBattleDefeat  = new OnBattleDefeatEvent();

        onBattleTickEnded  = new OnBattleTickEndedEvent();
        onHeatGaugeChanged = new OnHeatGaugeChangedEvent();

        onMinionSummoned = new OnMinionSummonedEvent();

        onFire   = new OnFireEvent();
        onSpin   = new OnSpinEvent();
        onReload = new OnReloadEvent();

        onOverheatStarted = new OnOverheatStartedEvent();
        onOverheatEnded   = new OnOverheatEndedEvent();

          loadedSkillQueue = new List<Skill>();
        unloadedSkillQueue = new List<Skill>();
    }
    public void Start()
    {
        ResetBattlePlayerController();
    }

    public void SetDreamflowNodeData(DreamflowNodeData nodeData)
    {
        ResetBattlePlayerController();

        node = nodeData;

        loadedSkillQueue.Clear();
        unloadedSkillQueue = new List<Skill>(MSGameInstance.Get().activeSkills);

        // Player Instance 
        playerInstance = new BattleFigureInstance(playerFigure, MSGameInstance.Get().GetPlayerHP(),
                                                                MSGameInstance.Get().GetPlayerArmor());
        playerInstance.onActivateSkill.AddListener(ActivateSkill);
        playerInstance.onFigureDeath.AddListener(OnFigureDeath);

        // Monster Instance 
        monsterInstances = new List<BattleFigureInstance>();
        for (var i = 0; i < 3; i++)
        {
            if (node.monsters[i] == null)
            {
                monsterInstances.Add(null);
                continue;
            }

            var instance = new BattleFigureInstance(node.monsters[i], BattleFigureInstanceType.Monster);
            instance.onActivateSkill.AddListener(ActivateSkill);
            instance.onDefenseEffectAdded.AddListener(OnDefenseEffectAdded);
            instance.onFigureDeath.AddListener(OnFigureDeath);

            monsterInstances.Add(instance);
        }

        minionInstances = new List<BattleFigureInstance>();
        minionInstances.Add(null);
        minionInstances.Add(null);
    }
    public void ResetBattlePlayerController()
    {
        heatGauge = 0; 
        isOverheated = false; 
        isBattleTickBegin = false; 
    }

    public void GetFigureInstances(out BattleFigureInstance player, out List<BattleFigureInstance> monsters, out List<BattleFigureInstance> summons)
    {
        player   =  playerInstance;
        monsters = monsterInstances;
        summons  =  minionInstances;
    }

    // Public
    public IEnumerator SetBattleResult(bool victory)
    {
        yield return new WaitForSeconds(0.5f);

        if (victory) 
            onBattleVictory.Invoke(); 
        else 
            onBattleDefeat. Invoke(); 
    }

    // Instance
    public void SummonMinion(MinionFigureData data)
    {
        if (data.formation == MinionFormationType.Front)
        {
            if (minionInstances[0] != null)
            {
                if (minionInstances[0].data == data)
                {
                    if (minionInstances[0].hp > 0)
                    {
                        minionInstances[0].SetHP   (data.defaultHP);
                        minionInstances[0].SetArmor(data.defaultArmor);

                        return;
                    }
                }
            }

            minionInstances[0] = new BattleFigureInstance(data, BattleFigureInstanceType.Minion);

            onMinionSummoned.Invoke(minionInstances[0]);
            minionInstances[0].onFigureSpawn.Invoke();
            minionInstances[0].onFigureDeath.AddListener(OnFigureDeath);
            minionInstances[0].onDefenseEffectAdded.AddListener(OnDefenseEffectAdded);
        }
        else
        {
            if (minionInstances[1] != null)
            {
                if (minionInstances[1].hp > 0)
                    return;
            }

            minionInstances[1] = new BattleFigureInstance(data, BattleFigureInstanceType.Minion);

            onMinionSummoned.Invoke(minionInstances[1]);
            minionInstances[1].onFigureSpawn.Invoke();
            minionInstances[1].onFigureDeath.AddListener(OnFigureDeath);
            minionInstances[1].onDefenseEffectAdded.AddListener(OnDefenseEffectAdded);
        }
    }

    public void SetTargetFigure(BattleFigureInstance instance)
    {
        targetInstance = instance;
    }
    public void ActivateSkill(BattleFigureInstance self, Skill skill)
    {
        var currentBattleFigureDatas = new List<BattleFigureInstance>();
        currentBattleFigureDatas.Add(playerInstance);
        currentBattleFigureDatas.AddRange(monsterInstances);
        currentBattleFigureDatas.AddRange( minionInstances);

        var skillTaskDatas = skill.GetSkillTasks();
        foreach (var instance in currentBattleFigureDatas)
        {
            if (instance == null)
                continue;

            for (var i = 0; i < skillTaskDatas.Count; i++)
            {
                var targetingType = skillTaskDatas[i].targetingType;
                if (targetingType == SkillTargetingType.None)
                {
                    if (!(skillTaskDatas[i] is SummonSkillTaskData)) 
                        continue;
                    
                    var taskData = skillTaskDatas[i] as SummonSkillTaskData;
                    SummonMinion(taskData.minion);
                }
                else if (targetingType == SkillTargetingType.Single)
                {
                    if (!IsSkillTaskTarget(self, instance, skillTaskDatas[i].targetFlag)) 
                        continue;
                    
                    // Self
                    if ((skillTaskDatas[i].targetFlag & SkillTargetFlag.Self) != 0)
                    {
                        if (instance == self)
                        {
                            StartCoroutine(instance.HitSkillTask(skillTaskDatas[i], multipleHitTimer));
                        }
                        continue;
                    }

                    // Target
                    if (self.instanceType == BattleFigureInstanceType.Player)
                    {
                        if (instance == targetInstance)
                            StartCoroutine(instance.HitSkillTask(skillTaskDatas[i], multipleHitTimer));
                    }
                    else if (self.instanceType == BattleFigureInstanceType.Monster)
                    {
                        if (minionInstances[0] != null)
                        {
                            if (minionInstances[0].hp > 0)
                            {
                                if (instance == minionInstances[0])
                                {
                                    StartCoroutine(instance.HitSkillTask(skillTaskDatas[i], multipleHitTimer));
                                }
                                continue;
                            }
                        }

                        if (playerFigure == null) 
                            continue;
                        
                        if (instance == playerInstance)
                            StartCoroutine(instance.HitSkillTask(skillTaskDatas[i], multipleHitTimer));
                    }
                }
                else if (targetingType == SkillTargetingType.Multiple)
                {
                    if (IsSkillTaskTarget(self, instance, skillTaskDatas[i].targetFlag))
                    {
                        StartCoroutine(instance.HitSkillTask(skillTaskDatas[i], multipleHitTimer));
                    }
                }
            }
        }
    }

    public bool IsSkillTaskTarget(BattleFigureInstance selfInstance, BattleFigureInstance targetInstance, SkillTargetFlag flag)
    {
        if (flag == SkillTargetFlag.Self)
        {
            if (selfInstance == targetInstance)
            {
                return true;
            }
            return false;
        }
        else if (flag == SkillTargetFlag.Enemy)
        {
            switch (selfInstance.instanceType)
            {
                case BattleFigureInstanceType.Player:
                case BattleFigureInstanceType.Minion:
                    return targetInstance.instanceType == BattleFigureInstanceType.Monster;
                
                case BattleFigureInstanceType.Monster:
                    return targetInstance.instanceType == BattleFigureInstanceType.Player ||
                           targetInstance.instanceType == BattleFigureInstanceType.Minion;
            }
        }
        else if (flag == SkillTargetFlag.Ally)
        {
            switch (selfInstance.instanceType)
            {
                case BattleFigureInstanceType.Player:
                case BattleFigureInstanceType.Minion:
                    return targetInstance.instanceType == BattleFigureInstanceType.Player ||
                           targetInstance.instanceType == BattleFigureInstanceType.Minion;
                
                case BattleFigureInstanceType.Monster:
                    return targetInstance.instanceType == BattleFigureInstanceType.Monster;
            }
        }

        return false;
    }

    public void OnDefenseEffectAdded(BattleFigureInstance instance)
    {
        instance.onFigureEffectAdded.Invoke(instance, defenseSkillEffect);
    }

    // Player
    public bool IsPlayerActionAvailable()
    {
        if (isOverheated)
            return false;

        if (isBattleTickBegin)
            return false;

        if (targetInstance.hp == 0)
            return false;

        if (playerInstance.hp == 0)
            return false;

        return true;
    }

    public void Fire()
    {
        if (!IsPlayerActionAvailable())
            return;

        if (loadedSkillQueue.Count == 0)
        {
            Reload();
            return;
        }

        Skill skill = loadedSkillQueue[0];
          loadedSkillQueue.Remove(skill);
        unloadedSkillQueue.Add(skill);

        int maxHeatGauge = MSGameInstance.Get().maxHeatGauge;
        heatGauge += skill.GetHeatValue();
        skill.ChangeExpCount(1);

        if (heatGauge >= maxHeatGauge)
        {
            heatGauge = maxHeatGauge;
            StartCoroutine(OverHeat());
        }

        // UI
        onHeatGaugeChanged.Invoke(heatGauge);
        onFire.Invoke();

        // Instance
        playerInstance.onFigureAttack.Invoke();
        ActivateSkill(playerInstance, skill);

        StartCoroutine(StartBattleTick());
    }
    public void Spin()
    {
        if (!IsPlayerActionAvailable())
            return; 

        if (loadedSkillQueue.Count == 0)
        {
            Reload();
            return;
        }

        var skill = loadedSkillQueue[0];
        loadedSkillQueue.Remove(skill);
        loadedSkillQueue.Add(skill);

        var coolingCount = MSGameInstance.Get().spinCoolingCount;
        heatGauge -= coolingCount;
        if (heatGauge < 0)
            heatGauge = 0;

        onHeatGaugeChanged.Invoke(heatGauge);
        onSpin.Invoke();

        StartCoroutine(StartBattleTick());
    }
    public void Reload()
    {
        if (!IsPlayerActionAvailable())
            return; 

        unloadedSkillQueue.AddRange(loadedSkillQueue);
        loadedSkillQueue.Clear();

        var count = MSGameInstance.Get().maxCylinderSlots;
        if (count > unloadedSkillQueue.Count)
            count = unloadedSkillQueue.Count;

          loadedSkillQueue = unloadedSkillQueue.GetRange(0, count);
        unloadedSkillQueue.RemoveRange(0, count);

        heatGauge -= MSGameInstance.Get().reloadCoolingCount;
        if (heatGauge < 0)
            heatGauge = 0;

        onHeatGaugeChanged.Invoke(heatGauge);
        onReload.Invoke(loadedSkillQueue);

        StartCoroutine(StartBattleTick());
    }

    IEnumerator StartBattleTick()
    {
        isBattleTickBegin = true;

        for (var i = 0; i < 3; i++)
        {
            if (monsterInstances[i] == null)
                continue;

            yield return new WaitForSeconds(battleTickTimer);
            monsterInstances[i].SetTickEnded();
        }
        for (var i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(battleTickTimer);

            if (minionInstances[i] == null)
                continue;

            minionInstances[i].SetTickEnded();
        }

        onBattleTickEnded.Invoke();
        isBattleTickBegin = false;
    }
    IEnumerator OverHeat()
    {
        isOverheated = true;
        onOverheatStarted.Invoke();

        var overheatGaugeValue = heatGauge;
        for (var i = 0; i < overheatGaugeValue; i++)
        {
            for (float t = 0; t < 1.0; t += Time.deltaTime / battleTickTimer)
            {
                yield return null;
            }

            heatGauge--;
            onHeatGaugeChanged.Invoke(heatGauge);

            StartCoroutine(StartBattleTick());
        }

        isOverheated = false;
        onOverheatEnded.Invoke();
    }

    public int GetLoadedSkillCount() { return loadedSkillQueue.Count; }

    // AI
    public void OnFigureAttack(BattleFigureInstance instance)
    {

    }
    public void OnFigureHit   (BattleFigureInstance instance)
    {

    }
    public void OnFigureDeath ()
    {
        // Result
        if (playerInstance.hp == 0)
            StartCoroutine(SetBattleResult(false));

        var isNoMoreEnemies = true;
        for (var i = 0; i < 3; i++)
        {
            if (monsterInstances[i] == null)
                continue;

            if (monsterInstances[i].hp <= 0) 
                continue;
            
            isNoMoreEnemies = false;
            break;
        }
        if (isNoMoreEnemies)
            StartCoroutine(SetBattleResult(true));
    }
}


// Instance
[System.Serializable] public class OnFigureSpawnEvent  : UnityEvent { }
[System.Serializable] public class OnFigureAttackEvent : UnityEvent { }
[System.Serializable] public class OnFigureHitEvent    : UnityEvent { }
[System.Serializable] public class OnFigureDeathEvent  : UnityEvent { }

[System.Serializable] public class OnFigureHPChangedEvent    : UnityEvent<int> { }
[System.Serializable] public class OnFigureArmorChangedEvent : UnityEvent<int> { }
[System.Serializable] public class OnFigureTimerChangedEvent : UnityEvent<int> { }

[System.Serializable] public class OnActivateSkillEvent : UnityEvent<BattleFigureInstance, Skill> { }

[System.Serializable] public class OnBuffAddedEvent   : UnityEvent<BuffInstance> { }
[System.Serializable] public class OnBuffRemovedEvent : UnityEvent<BuffInstance> { }

[System.Serializable] public class OnDefenseEffectAddedEvent : UnityEvent<BattleFigureInstance> { }
[System.Serializable] public class OnFigureEffectAddedEvent  : UnityEvent<BattleFigureInstance, SkillEffectData> { }

public class BattleFigureInstance
{
    public BattleFigureData         data;
    public BattleFigureInstanceType instanceType;

    public int hp;
    public int armor;
    public int timer;

    public List<BuffInstance> buffInstances;

    // Event
    public OnFigureSpawnEvent  onFigureSpawn;
    public OnFigureAttackEvent onFigureAttack;
    public OnFigureHitEvent    onFigureHit;
    public OnFigureDeathEvent  onFigureDeath;

    public OnFigureHPChangedEvent    onFigureHPChanged;
    public OnFigureArmorChangedEvent onFigureArmorChanged;
    public OnFigureTimerChangedEvent onFigureTimerChanged;

    public OnActivateSkillEvent onActivateSkill;

    public OnBuffAddedEvent   onBuffAdded;
    public OnBuffRemovedEvent onBuffRemoved;

    public OnDefenseEffectAddedEvent onDefenseEffectAdded;
    public OnFigureEffectAddedEvent  onFigureEffectAdded;

    // Init
    public BattleFigureInstance(BattleFigureData figureData, BattleFigureInstanceType type)
    {
        data = figureData;
        instanceType = type; 

        hp    = data.defaultHP; 
        armor = data.defaultArmor; 

        if (type == BattleFigureInstanceType.Monster) 
        {
            var monsterFigureData = figureData as MonsterFigureData; 
            timer = monsterFigureData.startTick; 
        }
        else
        {
            var minionFigureData = figureData as MinionFigureData; 
            timer = minionFigureData.startTick; 
        }
        InitializeCommonInstance(); 
    }
    public BattleFigureInstance(BattleFigureData playerData, int playerHP, int playerArmor)
    {
        data = playerData;
        instanceType = BattleFigureInstanceType.Player;

        hp    = playerHP;
        armor = playerArmor;
        timer = -1;

        InitializeCommonInstance();
    }
    public void InitializeCommonInstance()
    {
        buffInstances = new List<BuffInstance>();

        onFigureSpawn  = new OnFigureSpawnEvent();
        onFigureAttack = new OnFigureAttackEvent();
        onFigureHit    = new OnFigureHitEvent();
        onFigureDeath  = new OnFigureDeathEvent();

        onFigureHPChanged    = new OnFigureHPChangedEvent();
        onFigureArmorChanged = new OnFigureArmorChangedEvent();
        onFigureTimerChanged = new OnFigureTimerChangedEvent();

        onActivateSkill = new OnActivateSkillEvent();

        onBuffAdded   = new OnBuffAddedEvent();
        onBuffRemoved = new OnBuffRemovedEvent();

        onDefenseEffectAdded = new OnDefenseEffectAddedEvent();
        onFigureEffectAdded  = new OnFigureEffectAddedEvent();
    }

    // Hit
    public IEnumerator HitSkillTask(SkillTaskData task, float hitTimer)
    {
        if (hp <= 0) 
            yield break;
        
        switch (task)
        {
            case ChangeHPSkillTaskData taskData:
            {
                var hpTask = taskData;

                for (var j = 0; j < hpTask.hit; j++)
                {
                    if (j > 0)
                        yield return new WaitForSeconds(hitTimer);

                    ChangeHP(hpTask.modifier);

                    if (hp == 0)
                        break;
                }

                break;
            }
            case ChangeArmorSkillTaskData taskData:
            {
                var armorTask = taskData;
                ChangeArmor(armorTask.modifier);
                break;
            }
            case ChangeTimerSkillTaskData taskData:
            {
                var timerTask = taskData;
                ChangeTimer(timerTask.modifier);
                break;
            }
            case AddBuffSkillTaskData taskData:
            {
                var buffTask = taskData;
                AddBuffData(buffTask.buff);
                break;
            }
        }

        if (task.effect != null)
            onFigureEffectAdded.Invoke(this, task.effect);
    }

    // Buff
    public void AddBuffData(BuffData buffData)
    {
        var instance = new BuffInstance(buffData);
        buffInstances.Add(instance);

        onBuffAdded.Invoke(instance);
    }

    // Data
    public void SetHP(int value)
    {
        // HP
        hp = value;
        onFigureHPChanged.Invoke(hp);
    }
    public void SetArmor(int value)
    {
        // Armor
        armor = value;
        onFigureArmorChanged.Invoke(armor);
    }

    public void ChangeHP(int value)
    {
        // Damage Armor
        if (value < 0)
        {
            value += armor;
            if (value >= 0)
            {
                value = 0;
                onDefenseEffectAdded.Invoke(this);
            }
        }

        // HP
        hp += value;
        if (hp < 0)
            hp = 0;

        onFigureHPChanged.Invoke(hp);

        if (hp == 0)
            onFigureDeath.Invoke();
        else if (value < 0)
            onFigureHit.Invoke();
    }
    public void ChangeArmor(int value)
    {
        // Armor
        armor += value;
        if (armor < 0)
            armor = 0;

        onFigureArmorChanged.Invoke(armor);
    }
    public void ChangeTimer(int value)
    {
        // Armor
        timer += value;
        if (timer < 0)
            timer = 0;

        onFigureTimerChanged.Invoke(timer);
    }

    public void SetTickEnded()
    {
        if (hp == 0)
            return;

        if (timer > 0) 
            timer--;

        // Buff Count
        var tempBuffInstances = new List<BuffInstance>(buffInstances);
        for (var i = 0; i < tempBuffInstances.Count; i++)
        {
            tempBuffInstances[i].DecreaseBuffTimer();
            if (tempBuffInstances[i].data is ChangeHPBuffData)
            {
                var buffData = tempBuffInstances[i].data as ChangeHPBuffData;
                ChangeHP(buffData.value);
            }

            if (tempBuffInstances[i].timer == 0)
            {
                onBuffRemoved.Invoke(tempBuffInstances[i]);
                buffInstances.Remove(tempBuffInstances[i]);
            }
        }

        // Use Skill
        if (timer == 0)
        {
            var monsterFigureData = data as MonsterFigureData;
            if (monsterFigureData.skillData.Count > 0)
            {
                var skillData = monsterFigureData.skillData[0] as MonsterSkillData;
                timer = skillData.timer;

                onFigureAttack. Invoke();
                onActivateSkill.Invoke(this, new Skill(skillData));
            }
        }

        onFigureTimerChanged.Invoke(timer);
    }
}


[System.Serializable] public class OnBuffTimerChangedEvent : UnityEvent<int> { }

public class BuffInstance
{
    public BuffData data;
    public int timer;

    public OnBuffTimerChangedEvent onBuffTimerChanged;

    public BuffInstance(BuffData buffData)
    {
        data  = buffData;
        timer = buffData.timer;

        onBuffTimerChanged = new OnBuffTimerChangedEvent();
    }
    public void DecreaseBuffTimer()
    {
        timer--;
        onBuffTimerChanged.Invoke(timer);
    }
}
