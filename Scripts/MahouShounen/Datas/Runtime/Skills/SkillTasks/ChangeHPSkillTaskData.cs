using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill Task", menuName = "Data/Skill Task - Change HP", order = 61)]
public class ChangeHPSkillTaskData : SkillTaskData
{
    [Header("Skill Task - Change HP")]
    [SerializeField]
    private int _hit;
    public  int  hit
    {
        get { return _hit; }
    }
    
    [SerializeField]
    private int _modifier;
    public  int  modifier
    {
        get { return _modifier; }
    }
}
