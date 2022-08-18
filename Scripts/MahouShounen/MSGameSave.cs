using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MSGameSave
{
    public string playerName;
    public Date   playerBirthday;

    public Date currentDate;
    public int  currency;
    
    public List<Schedule> lastSchedules;
    public List<SchedulePreset> schedulePresets;

    public int playerStress;
    public List<PlayerStatSave> playerStats;
    public List<PlayerScheduleSave>  playerSchedules;

    public List<FriendshipSave> friendships;

    public List<ItemSave> items;

    public List<EventCountSave> eventCounts;
    public List<DreampostCountSave> dreampostCounts;

    public CasualwearType casualwear;
    public    CostumeType costume;
    
    public List<CasualwearType> unlockedCasualwears;
    public List<   CostumeType> unlockedCostumes;
    
    // BattleData
    public int playerHP;
    public int playerArmor;

    public List<SkillSave> playerSkills;
    public List<int>       activeSkillIndexes;

    // Game Instance
    public bool isNewGame;
    public LayoutMainMode layoutMainMode;


    public MSGameSave(MSGameInstance saveInstance)
    {
        playerName     = saveInstance.playerName;
        playerBirthday = saveInstance.playerBirthday;
        
        currentDate = saveInstance.currentDate;
        currency    = saveInstance.currency;

        lastSchedules = saveInstance.lastSchedules;
        schedulePresets = saveInstance.schedulePresets;

        playerStress    = saveInstance.playerStress;
        playerStats = new List<PlayerStatSave>();
        foreach (var pair in saveInstance.playerStats)
        {
            playerStats.Add(new PlayerStatSave(pair.Key, pair.Value));
        }
        playerSchedules = new List<PlayerScheduleSave>();
        foreach (var pair in saveInstance.playerSchedules)
        {
            playerSchedules.Add(new PlayerScheduleSave(pair.Key, pair.Value));
        }

        friendships = new List<FriendshipSave>();
        foreach (var pair in saveInstance.friendships)
        {
            friendships.Add(new FriendshipSave(pair.Key, pair.Value));
        }
        
        items = new List<ItemSave>();
        foreach (var item in saveInstance.items)
        {
            var save = new ItemSave(item);
            items.Add(save);
        }

        eventCounts = new List<EventCountSave>();
        foreach (var pair in saveInstance.eventCounts)
        {
            eventCounts.Add(new EventCountSave(pair.Key, pair.Value));
        }
        dreampostCounts = new List<DreampostCountSave>();
        foreach (var pair in saveInstance.dreampostCounts)
        {
            dreampostCounts.Add(new DreampostCountSave(pair.Key, pair.Value));
        }
        
        casualwear = saveInstance.casualwear;
           costume = saveInstance.costume;
           
        unlockedCasualwears = saveInstance.unlockedCasualwears;
           unlockedCostumes = saveInstance.unlockedCostumes;

        // BattleData
        playerHP    = saveInstance.playerHP;
        playerArmor = saveInstance.playerArmor;

        playerSkills = new List<SkillSave>();
        foreach (var skill in saveInstance.playerSkills)
        {
            var save = new SkillSave(skill);
            playerSkills.Add(save);
        }
        
        activeSkillIndexes = new List<int>();
        foreach (var skill in saveInstance.activeSkills)
        {
            var index = saveInstance.playerSkills.IndexOf(skill);
            activeSkillIndexes.Add(index);
        }
        
        // Game Instance
        isNewGame = false;
        layoutMainMode = saveInstance.layoutMainMode;
    }

    public void Save()
    {
        var path = Path.Combine(Application.persistentDataPath, "GameSave.json");
        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, json);
    }
    public void Load(MSGameInstance instance)
    {
        instance.playerName     = playerName;
        instance.playerBirthday = playerBirthday;
        
        instance.currentDate = currentDate;
        instance.currency    = currency;
        
        instance.lastSchedules = lastSchedules;
        instance.schedulePresets = schedulePresets;
        
        instance.playerStress = playerStress;
        foreach (var save in playerStats)
        {
            if (instance.playerStats.ContainsKey(save.stat))
                instance.playerStats[save.stat] = save.statBase;
            else
                instance.playerStats.Add(save.stat, save.statBase);
        }
        foreach (var save in playerSchedules)
        {
            if (instance.playerSchedules.ContainsKey(save.type))
                instance.playerSchedules[save.type] = save.count;
            else
                instance.playerSchedules.Add(save.type, save.count);
        }
        
        foreach (var save in playerSchedules)
        {
            if (instance.playerSchedules.ContainsKey(save.type))
                instance.playerSchedules[save.type] = save.count;
            else
                instance.playerSchedules.Add(save.type, save.count);
        }
        
        instance.items = new List<Item>();
        foreach (var itemSave in items)
        {
            var item = new Item(itemSave);
            instance.items.Add(item);
        }
        
        foreach (var save in eventCounts)
        {
            if (instance.eventCounts.ContainsKey(save.id))
                instance.eventCounts[save.id] = save.count;
            else
                instance.eventCounts.Add(save.id, save.count);
        }
        foreach (var save in dreampostCounts)
        {
            if (instance.dreampostCounts.ContainsKey(save.id))
                instance.dreampostCounts[save.id] = save.count;
            else
                instance.dreampostCounts.Add(save.id, save.count);
        }
        
        instance.casualwear = casualwear;
        instance.costume    = costume;

        instance.unlockedCasualwears = unlockedCasualwears;
        instance.unlockedCostumes    = unlockedCostumes;
        
        // BattleData
        instance.playerHP    = playerHP;
        instance.playerArmor = playerArmor;
        
        instance.playerSkills = new List<Skill>();
        foreach (var skill in playerSkills)
        {
            var playerSkill = new Skill(skill);
            instance.playerSkills.Add(playerSkill);
        }
        
        instance.activeSkills = new List<Skill>();
        foreach (var index in activeSkillIndexes)
        {
            var activeSkill = instance.playerSkills[index];
            instance.activeSkills.Add(activeSkill);
        }
        
        // Game Instance
        instance.isNewGame = false;
        instance.layoutMainMode = layoutMainMode;
    }
}


[System.Serializable]
public class PlayerStatSave
{
    public Stat stat;
    public StatBase statBase;

    public PlayerStatSave(Stat targetStat, StatBase targetStatBase)
    {
        stat = targetStat;
        statBase = targetStatBase;
    }
}
[System.Serializable]
public class PlayerScheduleSave
{
    public Schedule type;
    public int count;
    
    public PlayerScheduleSave(Schedule targetType, int targetCount)
    {
        type = targetType;
        count = targetCount;
    }
}

[System.Serializable]
public class SkillSave
{
    public int       id;
    public SkillRank rank;
    public int       exp;

    public SkillSave(Skill skill)
    {
        id   = skill.data.id;
        rank = skill.rank;
        exp  = skill.exp;
    }
}

[System.Serializable]
public class ItemSave
{
    public int id;
    public int count;

    public ItemSave(Item item)
    {
        id    = item.itemData.id;
        count = item.itemCount;
    }
}

[System.Serializable]
public class FriendshipSave
{
    public FriendshipType type;
    public int count;
    
    public FriendshipSave(FriendshipType targetType, int targetCount)
    {
        type  = targetType;
        count = targetCount;
    }
}
[System.Serializable]
public class EventCountSave
{
    public int id;
    public int count;
    
    public EventCountSave(int targetID, int targetCount)
    {
        id = targetID;
        count = targetCount;
    }
}
[System.Serializable]
public class DreampostCountSave
{
    public int id;
    public int count;
    
    public DreampostCountSave(int targetID, int targetCount)
    {
        id = targetID;
        count = targetCount;
    }
}
