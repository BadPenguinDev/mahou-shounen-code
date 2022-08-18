using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Figure", menuName = "Data/Battle Figure", order = 100)]
public class BattleFigureData : ScriptableObject
{
    [Header("Element Transform")]
    [SerializeField]
    private Vector2 _spriteSize;
    public  Vector2  spriteSize
    {
        get { return _spriteSize; }
    }
    
    [SerializeField]
    private Vector2 _buttonPivot;
    public  Vector2  buttonPivot
    {
        get { return _buttonPivot; }
    }
    
    [SerializeField]
    private Vector2 _buttonSize;
    public  Vector2  buttonSize
    {
        get { return _buttonSize; }
    }
    

    [Header("Animation - Idle")]
    [SerializeField] 
    private float _idleTimer;
    public  float  idleTimer
    {
        get { return _idleTimer; }
    }

    [SerializeField] 
    private List<Sprite> _idleSprite;
    public  List<Sprite>  idleSprite
    {
        get { return _idleSprite; }
    }


    [Header("Animation - Attack")]
    [SerializeField] 
    private float _attackTimer;
    public  float  attackTimer
    {
        get { return _attackTimer; }
    }

    [SerializeField] 
    private List<Sprite> _attackSprite;
    public  List<Sprite>  attackSprite
    {
        get { return _attackSprite; }
    }


    [Header("Animation - Hit")]
    [SerializeField] 
    private float _hitTimer;
    public  float  hitTimer
    {
        get { return _hitTimer; }
    }

    [SerializeField] 
    private List<Sprite> _hitSprite;
    public  List<Sprite>  hitSprite
    {
        get { return _hitSprite; }
    }


    [Header("Animation - Death")]
    [SerializeField] 
    private float _deathTimer;
    public  float  deathTimer
    {
        get { return _deathTimer; }
    }

    [SerializeField] 
    private List<Sprite> _deathSprite;
    public  List<Sprite>  deathSprite
    {
        get { return _deathSprite; }
    }


    [Header("Data - Stats")]
    [SerializeField]
    private int _defaultHP;
    public  int  defaultHP
    {
        get { return _defaultHP; }
    }

    [SerializeField]
    private int _defaultArmor;
    public  int  defaultArmor
    {
        get { return _defaultArmor; }
    }

}
