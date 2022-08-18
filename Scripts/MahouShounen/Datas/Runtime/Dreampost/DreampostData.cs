using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Dreampost", menuName = "Data/Dreampost", order = 140)]
public class DreampostData : ScriptableObject
{
    [Header("Dreampost")]
    [SerializeField]
    private int _id;
    public  int  id
    {
        get { return _id; }
    }

    [SerializeField]
    private LocalizedString _postName;
    public  LocalizedString  postName
    {
        get { return _postName; }
    }

    [SerializeField]
    private LocalizedString _postDescription;
    public  LocalizedString  postDescription
    {
        get { return _postDescription; }
    }
    
    [SerializeField]
    private List<DreamflowRewardData> _rewards;
    public  List<DreamflowRewardData>  rewards
    {
        get { return  _rewards; }
    }


    [Header("Dreamflow")]
    [SerializeField]
    private DreamflowNodeData _homeNode;
    public  DreamflowNodeData  homeNode
    {
        get { return  _homeNode; }
    }

    [SerializeField]
    private DreamflowNodeData _bossNode;
    public  DreamflowNodeData  bossNode
    {
        get { return  _bossNode; }
    }

    [SerializeField]
    private List<DreamflowColumnData> _nodeColumns;
    public  List<DreamflowColumnData>  nodeColumns
    {
        get { return  _nodeColumns; }
    }


    [Header("Event")]
    [SerializeField]
    private EventData _eventBegin;
    public  EventData  eventBegin
    {
        get { return _eventBegin; }
    }

    [SerializeField]
    private EventData _eventSuccess;
    public  EventData  eventSuccess
    {
        get { return _eventSuccess; }
    }

    [SerializeField]
    private EventData _eventFailed;
    public  EventData  eventFailed
    {
        get { return _eventFailed; }
    }


    public virtual Sprite GetPortraitMini()
    {
        return null;
    }
}

[System.Serializable]
public class DreamflowColumnData
{
    [SerializeField]
    private List<DreamflowNodeData> _nodes;
    public  List<DreamflowNodeData>  nodes
    {
        get { return _nodes; }
    }
}

[System.Serializable]
public class DreamflowNodeData
{
    [SerializeField]
    private bool _isActive;
    public  bool  isActive
    {
        get { return _isActive; }
    }

    [SerializeField]
    private Schedule _schedule;
    public  Schedule  schedule
    {
        get { return _schedule; }
    }

    [SerializeField]
    private List<BattleFigureData> _monsters;
    public  List<BattleFigureData>  monsters
    {
        get { return _monsters; }
    }
    
    [SerializeField]
    private List<bool> _connections;
    public  List<bool>  connections
    {
        get { return _connections; }
    }
}

[System.Serializable]
public class DreamflowRewardData
{
    [SerializeField]
    private ItemData _item;
    public  ItemData  item
    {
        get { return _item; }
    }
    
    [SerializeField]
    private int _amount;
    public  int  amount
    {
        get { return _amount; }
    }
}
