using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum SkillTargetFlag    { Self = 1 << 0, Enemy = 1 << 1, Ally = 1 << 2 }
public enum SkillTargetingType { None, Single, Multiple }

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task", order = 60)]
public class SkillTaskData : ScriptableObject
{
    [Header("Skill Task - Common")]
    [SerializeField]
    private int _id;
    public  int  id
    {
        get { return _id; }
    }

    [SerializeField]
    private SkillTargetFlag _targetFlag;
    public  SkillTargetFlag  targetFlag
    {
        get { return _targetFlag; }
    }

    [SerializeField]
    private SkillTargetingType _targetingType;
    public  SkillTargetingType  targetingType
    {
        get { return _targetingType; }
    }

    [SerializeField]
    private SkillEffectData _effect;
    public  SkillEffectData  effect
    {
        get { return _effect; }
    }
}

[System.Serializable]
public class SkillTaskEffectData
{

}