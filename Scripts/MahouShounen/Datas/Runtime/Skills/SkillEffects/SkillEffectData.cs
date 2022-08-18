using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectData : ScriptableObject
{
    [Header("Skill Effect")]
    [SerializeField]
    private Vector2 _size;
    public  Vector2  size
    {
        get { return _size; }
    }

    [SerializeField]
    private Vector2 _pivot;
    public  Vector2  pivot
    {
        get { return _pivot; }
    }

    [SerializeField]
    private List<Sprite> _sprites;
    public  List<Sprite>  sprites
    {
        get { return _sprites; }
    }

    [SerializeField]
    private float _timer;
    public  float  timer
    {
        get { return _timer; }
    }

    [SerializeField]
    private bool _isRandom;
    public  bool  isRandom
    {
        get { return _isRandom; }
    }

    public virtual Vector2 GetEffectPosition(Vector2 position)
    {
        if (isRandom)
            position += new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));

        return position + pivot;
    }
}
