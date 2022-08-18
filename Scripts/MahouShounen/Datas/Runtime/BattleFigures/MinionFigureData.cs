using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Figure", menuName = "Data/Minion Figure", order = 103)]
public class MinionFigureData : BattleFigureData
{
    [Header("Animation - Spawn")]
    [SerializeField] 
    private float _spawnTimer;
    public  float  spawnTimer
    {
        get { return _spawnTimer; }
    }

    [SerializeField] 
    private List<Sprite> _spawnSprite;
    public  List<Sprite>  spawnSprite
    {
        get { return _spawnSprite; }
    }

    [Header("Minion")]
    [SerializeField]
    private MinionFormationType _formation;
    public  MinionFormationType  formation
    {
        get { return _formation; }
    }

    [Header("Skills - Minion")]
    [SerializeField]
    private List<SkillData> _skillData;
    public  List<SkillData>  skillData
    {
        get { return _skillData; }
    }

    [SerializeField]
    private int _startTick;
    public  int  startTick
    {
        get { return _startTick; }
    }
}
