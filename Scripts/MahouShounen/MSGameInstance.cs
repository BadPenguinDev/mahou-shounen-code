using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


[System.Serializable] public class OnDateChangedEvent : UnityEvent<Date> { }
[System.Serializable] public class OnCurrencyChangedEvent : UnityEvent<int> { }
[System.Serializable] public class OnStressChangedEvent : UnityEvent<int> { }
[System.Serializable] public class OnStatChangedEvent : UnityEvent<Stat, StatBase> { }

[System.Serializable] public class OnMusicVolumeChangedEvent : UnityEvent<int> { }
[System.Serializable] public class OnSoundVolumeChangedEvent : UnityEvent<int> { }


[System.Serializable]
public class MSGameInstance
{
    public static MSGameInstance GameInstance;

    public string playerName;
    public Date   playerBirthday;

    public Date currentDate;
    public int  currency;
    
    public List<Schedule> lastSchedules;
    public List<SchedulePreset> schedulePresets;

    public int playerStress;
    public Dictionary<Stat, StatBase> playerStats;
    public Dictionary<Schedule, int>  playerSchedules;

    public Dictionary<FriendshipType, int> friendships;

    public List<Item> items;

    public Dictionary<int, int> eventCounts;
    public Dictionary<int, int> dreampostCounts;

    public CasualwearType casualwear;
    public    CostumeType costume;

    public List<CasualwearType> unlockedCasualwears;
    public List<   CostumeType> unlockedCostumes;
    
    // BattleData
    public int playerHP;
    public int playerArmor;

    public int maxCylinderSlots;
    public int maxHeatGauge;
    public int spinCoolingCount;
    public int reloadCoolingCount;
    
    public List<Skill> playerSkills;
    public List<Skill> activeSkills;

    // Game Instance
    public bool isNewGame;
    public LayoutMainMode layoutMainMode;

    // Preferences
    public MSGamePreferences preferences;

    // Delegates
    public OnDateChangedEvent onDateChanged;
    
    public OnCurrencyChangedEvent onCurrencyChanged;
    public OnStressChangedEvent   onStressChanged;
    public OnStatChangedEvent     onStatChanged;

    public OnMusicVolumeChangedEvent onMusicVolumeChanged;
    public OnSoundVolumeChangedEvent onSoundVolumeChanged;


    // Access
    public static MSGameInstance Get()
    {
        if (GameInstance != null) 
            return GameInstance;
        
        GameInstance = new MSGameInstance();
        GameInstance.InitGameInstance();

        return GameInstance;
    }

    // Init
    public void InitGameInstance()
    {
        var initData = MSDataManager.Get().globalInitializeData;
        isNewGame = true;
        
        playerName     = initData.playerName.GetLocalizedString();
        playerBirthday = initData.playerBirthday;

        playerStats     = new Dictionary<Stat, StatBase>();
        playerSchedules = new Dictionary<Schedule,  int>();
        
        currency = 200;
        lastSchedules = new List<Schedule>();
        schedulePresets = new List<SchedulePreset>();

        foreach (Stat stat in System.Enum.GetValues(typeof(Stat)))
        {
            playerStats.Add(stat, new StatBase());
        }
        foreach (Schedule schedule in System.Enum.GetValues(typeof(Schedule)))
        {
            playerSchedules.Add(schedule, 0);
        }

        friendships = new Dictionary<FriendshipType, int>();
        foreach (FriendshipType type in System.Enum.GetValues(typeof(FriendshipType)))
        {
            friendships.Add(type, (int)type * 80);
        }

        items = new List<Item>(initData.items);

            eventCounts = new Dictionary<int, int>();
        dreampostCounts = new Dictionary<int, int>();

        currentDate = initData.currentDate;
        
        casualwear = CasualwearType.SchoolUniform;
        costume    = CostumeType.MagicalGirl;

        unlockedCasualwears = new List<CasualwearType>();
        unlockedCostumes    = new List<CostumeType>();
        
        unlockedCasualwears.Add(casualwear);
        unlockedCostumes   .Add(costume);
        
        // DEBUG
        unlockedCasualwears.Add(CasualwearType.Butler);
        unlockedCasualwears.Add(CasualwearType.Admiral);
        unlockedCostumes.Add(CostumeType.MaidDress);
        unlockedCostumes.Add(CostumeType.Sailor);

        #region Skill Datas
        playerSkills = new List<Skill>();
        foreach (var skill in initData.playerSkills)
        {
            playerSkills.Add(new Skill(skill));
        }

        var maxSkillIndex = initData.maxCylinderSlots < playerSkills.Count
            ? initData.maxCylinderSlots
            : playerSkills.Count;
        
        activeSkills = new List<Skill>();
        for (var i = 0; i < maxSkillIndex; i++)
        {
            activeSkills.Add(playerSkills[i]);
        }
        #endregion

        #region Battle Data
        playerHP    = initData.playerHP;
        playerArmor = initData.playerArmor;
        
        maxCylinderSlots   = initData.maxCylinderSlots;
        maxHeatGauge       = initData.maxHeatGauge;
        spinCoolingCount   = initData.spinCoolingCount;
        reloadCoolingCount = initData.reloadCoolingCount;
        #endregion

        preferences = new MSGamePreferences();
        preferences.Load();

        onDateChanged = new OnDateChangedEvent();
        
        onCurrencyChanged = new OnCurrencyChangedEvent();
        onStatChanged     = new OnStatChangedEvent();
        onStressChanged   = new OnStressChangedEvent();
        
        onMusicVolumeChanged = new OnMusicVolumeChangedEvent();
        onSoundVolumeChanged = new OnSoundVolumeChangedEvent();
    }
    
    // Save & Load
    public void SaveGameData()
    {
        var gameSave = new MSGameSave(this);
        gameSave.Save();
    }
    public void LoadGameData()
    {
        var path = Path.Combine(Application.persistentDataPath, "GameSave.json");
        if (!File.Exists(path))
        {
            InitGameInstance();
            SaveGameData();
        } 
        else 
        {
            var json = File.ReadAllText(path);
            var gameSave = JsonUtility.FromJson<MSGameSave>(json);
            gameSave.Load(this);
        }
    }

    public void OnLayoutModeChanged(LayoutMainMode mode)
    {
        layoutMainMode = mode;
        SaveGameData();
    }
    
    public void OnDailyScheduleRoutineStarted()
    {
        currentDate.SetNextDay();
        onDateChanged.Invoke(currentDate);
    }
    public void OnDailyScheduleFinished(Schedule schedule)
    {
        // Schedule Data
        playerSchedules[schedule] += 1;
        
        var targetScheduleData = MSDataManager.Get().scheduleDatas.Find(x => x.schedule == schedule);
        var grade = GetScheduleGrade(targetScheduleData);
        foreach (var stat in targetScheduleData.GetStats(grade))
        {
            if (!playerStats[stat.type].isLocked)
            {
                var minimumStatLimit = MSCommon.GetStatRankStartValue(playerStats[stat.type].rank);
                
                playerStats[stat.type].value += stat.modifier;
                if (playerStats[stat.type].value < minimumStatLimit)
                    playerStats[stat.type].value = minimumStatLimit;
            }
            onStatChanged.Invoke(stat.type, playerStats[stat.type]);
        }
        
        playerStress += targetScheduleData.GetStress(grade);
        if (playerStress < 0)
            playerStress = 0;
        
        currency += targetScheduleData.GetCost(grade);
        
        onStressChanged.Invoke(playerStress);
        onCurrencyChanged.Invoke(currency);

        // Friendships
        var friendship = MSDataManager.Get().FindCharacterTypeByDailySchedule(currentDate.day, schedule);
        friendships[friendship] += 1;

        // Finish
        currentDate.SetNextDay();
        onDateChanged.Invoke(currentDate);
    }

    // Stat
    public StatBase GetStatBase(Stat stat)
    {
        return playerStats[stat];
    }

    // Items
    public void ChangeItemCount(Item targetItem)
    {
        var item = GetItemByItemData(targetItem.itemData);
        if (item == null)
        {
            if (targetItem.itemCount > 0)
                items.Add(targetItem);
        }
        else
        {
            item.ChangeItemCount(targetItem.itemCount);
            if (item.itemCount <= 0)
                items.Remove(item);
        }
    }
    public Item GetItemByItemData(ItemData itemData)
    {
        foreach (var item in items)
        {
            if (item.itemData == itemData)
                return item;
        }
        return null;
    }
    public int GetItemCountByItemData(ItemData itemData)
    {
        foreach (var item in items)
        {
            if (item.itemData == itemData)
                return item.itemCount;
        }
        return 0;
    }

    // Event Counts
    public void AddEventCount(EventData eventData)
    {
        if (eventCounts == null)
            eventCounts = new Dictionary<int, int>();

        if (eventCounts.ContainsKey(eventData.id))
            eventCounts[eventData.id] += 1;
        else
            eventCounts.Add(eventData.id, 1);
    } 

    // Schedule
    public ScheduleGrade GetScheduleGrade(ScheduleData data)
    {
        if (data == null) 
            return ScheduleGrade.Beginner;

        if (data.type == ScheduleType.Others) 
            return ScheduleGrade.Beginner;
        
        return !playerSchedules.ContainsKey(data.schedule) ? ScheduleGrade.Beginner : MSCommon.GetScheduleGradeByCount(playerSchedules[data.schedule]);
    }
    public bool IsScheduleAvailable(ScheduleData data)
    {
        if (data.type == ScheduleType.Class &&
            currency <= 0)
            return false;
        else
            return true;
    }

    // Schedule preset
    public void AddSchedulePreset(SchedulePreset targetPreset)
    {
        var ids = new List<int>();
        foreach (var preset in schedulePresets)
        {
            ids.Add(preset.id);
        }

        var id = 0;
        while (true)
        {
            id++;
            if (!ids.Contains(id))
                break;
        }

        targetPreset.id = id;
        schedulePresets.Add(targetPreset);
    }

    public void RemoveSchedulePreset(int id)
    {
        var preset = schedulePresets.Find(x => x.id == id);
        if (preset != null)
            schedulePresets.Remove(preset);
    }
    
    // Dreampost
    public void AddDreampostCount(DreampostData dreampostData)
    {
        if (dreampostCounts == null)
            dreampostCounts = new Dictionary<int, int>();

        if (dreampostCounts.ContainsKey(dreampostData.id))
            dreampostCounts[dreampostData.id] += 1;
        else
            dreampostCounts.Add(dreampostData.id, 1);
    }
    public List<DreampostData> GetAvailableDreampostDatas(int count)
    {
        var availableDreamposts = new List<DreampostData>();

        // Get Friendship Data
        var freindshipDreampostDatas = new List<FriendshipDreampostData>(MSDataManager.Get().friendshipDreampostDatas);
        var availableFriendshipPosts = new List<FriendshipDreampostData>();
        foreach (var currentData in freindshipDreampostDatas)
        {
            if (!dreampostCounts.ContainsKey(currentData.id) &&
                eventCounts.ContainsKey(currentData.requiredEvent.id))
            {
                availableFriendshipPosts.Add(currentData);
            }
        }
        availableDreamposts.AddRange(availableFriendshipPosts);

        // Get Random Data
        List<RandomDreampostData> randomDreampostDatas = new List<RandomDreampostData>(MSDataManager.Get().randomDreampostDatas);
        availableDreamposts.AddRange(randomDreampostDatas);

        // Limit
        if (availableDreamposts.Count > count)
        {
            availableDreamposts = availableDreamposts.GetRange(0, count);
        }
        else
        {
            for (int i = availableDreamposts.Count; i < count; i++)
            {
                availableDreamposts.Add(null);
            }
        }
        availableDreamposts.Sort((a, b) => 1 - 2 * Random.Range(0, 2));

        return availableDreamposts;
    }

    // Free Time Events
    public Dictionary<FriendshipType, WeekendEventData> GetEventOptionsByCondition(Spot spot)
    {
        var eventOptions = new Dictionary<FriendshipType, WeekendEventData>();

        foreach (var characterData in MSDataManager.Get().characterDatas)
        {
            eventOptions.Add(characterData.friendshipType, FindCurrentWeekendEvent(characterData));
        }

        return eventOptions;
    }
    public WeekendEventData FindCurrentWeekendEvent(CharacterData characterData)
    {
        WeekendEventData currentEventData = null;
        foreach (var eventData in characterData.weekendEvents)
        {
            if (!IsEventAvailable(eventData)) 
                continue;
            
            if (currentEventData == null)
                currentEventData = eventData;

            else if (currentEventData.priority > eventData.priority)
                currentEventData = eventData;
        }
        return currentEventData;
    }
    public bool IsEventAvailable(EventData eventData)
    {
        foreach (var condition in eventData.conditions)
        {
            var status = true;

            switch (condition.type)
            {
                case EventConditionType.Date:
                    status = Date.Compare(currentDate, new Date(condition.month, condition.week, condition.day), condition.constantCondition);
                    break;
                case EventConditionType.Friendship:
                    status = MSCommon.CompareConstant(friendships[condition.friendship], condition.friendshipValue, condition.constantCondition);
                    break;
                case EventConditionType.Schedule:
                    status = MSCommon.CompareConstant(playerSchedules[condition.schedule], condition.scheduleValue, condition.constantCondition);
                    break;
                case EventConditionType.Stat:
                    status = MSCommon.CompareConstant(playerStats[condition.stat].value, condition.statValue, condition.constantCondition);
                    break;
                case EventConditionType.Event:
                    status = !eventCounts.ContainsKey(eventData.id);
                    break;
            } 

            if (status == false)
                return false;
        }

        return true;
    }

    // Battle - Player
    public int GetPlayerHP()    { return playerHP; }
    public int GetPlayerArmor() { return playerArmor; }
    
    // Settings
    public void SetMusicVolume(int volume)
    {
        preferences.volumeMusic = volume;
        onMusicVolumeChanged.Invoke(volume);
        
        preferences.Save();
    }
    public void SetSoundVolume(int volume)
    {
        preferences.volumeSound = volume;
        onSoundVolumeChanged.Invoke(volume);
        
        preferences.Save();
    }
}

[System.Serializable]
public class StatBase
{
    public StatRank rank;
    public int  value;
    public bool isLocked;

    public StatBase()
    {
        rank = StatRank.D;
        value = 0;
        isLocked = false;
    }
}

[System.Serializable]
public class SchedulePreset
{
    public int id;
    public List<Schedule> schedules;

    public int Count => schedules.Count;

    public Schedule this[int index] => schedules[index];

    public SchedulePreset()
    {
        schedules = new List<Schedule>();
    }
    public void Add(Schedule schedule)
    {
        schedules.Add(schedule);
    }
}
