using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Skill", menuName = "Data/Skill", order = 40)]
public class SkillData : ScriptableObject
{
    [SerializeField]
    private int _id;
    public  int  id
    {
        get { return _id; }
    }

    [SerializeField]
    private List<SkillTaskData> _tasks;
    public  List<SkillTaskData>  tasks
    {
        get { return _tasks; }
    }
}

public class OnSkillUpgradedEvent : UnityEvent<SkillRank> { }
[System.Serializable]
public class Skill
{
    public SkillData data;
    public SkillRank rank;
    public int       exp;
    public OnSkillUpgradedEvent onSkillUpgraded;
    
    public Skill(Skill skill)
    {
        data = skill.data;
        rank = skill.rank;
        exp  = skill.exp;

        onSkillUpgraded = new OnSkillUpgradedEvent();
    }
    public Skill(SkillSave save)
    {
        data = MSDataManager.Get().playerSkillDatas.Find(x => x.id == save.id);
        rank = save.rank;
        exp  = save.exp;

        onSkillUpgraded = new OnSkillUpgradedEvent();
    }
    public Skill(SkillData skillData)
    {
        data = skillData;
        rank = SkillRank.D;
        exp  = 0;

        onSkillUpgraded = new OnSkillUpgradedEvent();
    }
    public Skill(SkillData skillData, SkillRank skillRank, int skillExp)
    {
        data = skillData;
        rank = skillRank;
        exp  = skillExp;

        onSkillUpgraded = new OnSkillUpgradedEvent();
    }

    public void ChangeExpCount(int value)
    {
        if (rank == SkillRank.S)
            return;
        
        var playerSkillData = data as PlayerSkillData;
        foreach (var data in playerSkillData.rankDatas)
        {
            if (data.rank != rank) 
                continue;
            
            if (exp >= data.EXP) 
                continue;
            
            exp += value;
            return;
        }
    }
    public void UpgradeSkill()
    {
        if (rank > 0)
            rank--;

        exp = 0;
        
        onSkillUpgraded.Invoke(rank);
    }

    public List<SkillTaskData> GetSkillTasks()
    {
        if (data is MonsterSkillData)
        {
            var monsterSkillData = data as MonsterSkillData;
            return monsterSkillData.tasks;
        }
        else if (data is PlayerSkillData)
        {
            var playerSkillData = data as PlayerSkillData;
            foreach (var skillData in playerSkillData.rankDatas)
            {
                if (skillData.rank == rank)
                    return skillData.tasks;
            }
        }
        return null;
    }
    public int GetHeatValue()
    {
        if (!(data is PlayerSkillData)) 
            return -1;
        
        var playerSkillData = data as PlayerSkillData;
        foreach (var skillData in playerSkillData.rankDatas)
        {
            if (skillData.rank == rank)
                return skillData.heatValue;
        }
        return -1;
    }
    public int GetExpValue()
    {
        if (!(data is PlayerSkillData)) 
            return -1;
        
        var playerSkillData = data as PlayerSkillData;
        foreach (var skillData in playerSkillData.rankDatas)
        {
            if (skillData.rank == rank)
                return skillData.EXP;
        }
        return -1;
    }
    public List<SkillUpgradeRequirementData> GetUpgradeRequirements()
    {
        if (!(data is PlayerSkillData)) 
            return null;
        
        var playerSkillData = data as PlayerSkillData;
        foreach (var skillData in playerSkillData.rankDatas)
        {
            if (skillData.rank == rank)
                return skillData.upgradeRequirements;
        }
        return null;
    }
}

