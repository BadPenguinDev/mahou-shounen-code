using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class OnDailyAnimationEndedEvent : UnityEvent { }

public class ElementDailyAnimation : ElementBase
{
    [SerializeField]
    private GameObject _defaultFigureSpriteObject;
    public  GameObject  defaultFigureSpriteObject
    {
        get => _defaultFigureSpriteObject;
        set => _defaultFigureSpriteObject = value;
    }
    
    [SerializeField]
    private List<DailyAnimationFigureData> _figureDatas;
    public  List<DailyAnimationFigureData>  figureDatas
    {
        get => _figureDatas;
        set => _figureDatas = value;
    }

    public OnDailyAnimationEndedEvent onDailyAnimationEnded;

    public void SetDailyAnimationEnded()
    {
        onDailyAnimationEnded.Invoke();
    }

    public void SetDailyAnimationFigure(FriendshipType type)
    {
        foreach (var figureData in figureDatas)
        {
            figureData.figureSpriteObject.SetActive(false);
        }
        
        if (type == FriendshipType.None)
        {
            defaultFigureSpriteObject.SetActive(true);
            return;
        }

        var targetData = figureDatas.Find(x => x.friendshipType == type);
        var targetObject = targetData.figureSpriteObject;
        
        targetObject.SetActive(true);
        defaultFigureSpriteObject.SetActive(false);
    }
}

[System.Serializable]
public class DailyAnimationFigureData
{
    [SerializeField]
    private FriendshipType _friendshipType;
    public  FriendshipType  friendshipType
    {
        get { return  _friendshipType; }
        set { _friendshipType = value; }
    }
    
    [SerializeField]
    private GameObject _figureSpriteObject;
    public  GameObject  figureSpriteObject
    {
        get { return  _figureSpriteObject; }
        set { _figureSpriteObject = value; }
    }
}