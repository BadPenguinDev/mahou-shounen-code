using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dreampost", menuName = "Data/Dreampost - Friendship", order = 140)]
public class FriendshipDreampostData : DreampostData
{
    [Header("Dreampost - Friendship")]
    [SerializeField]
    private FriendshipType _type;
    public  FriendshipType  type
    {
        get { return _type; }
    }
    
    [SerializeField]
    private EventData _requiredEvent;
    public  EventData  requiredEvent
    {
        get { return _requiredEvent; }
    }

    public override Sprite GetPortraitMini()
    {
        CharacterData data = MSDataManager.Get().characterDatas.Find(x => x.friendshipType == type);
        return data.iconSprite;
    }
}
