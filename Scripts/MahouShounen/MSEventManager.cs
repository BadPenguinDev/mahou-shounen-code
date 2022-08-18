using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable] public class OnBeginEventEvent : UnityEvent<EventData> { }
[System.Serializable] public class   OnEndEventEvent : UnityEvent<EventData> { }

public class MSEventManager : MonoBehaviour
{
    // Access
    public static MSEventManager eventManager;
    public static MSEventManager Get()
    {
        return eventManager;
    }

    [SerializeField] 
    private EventData _currentEventData;
    public  EventData  currentEventData
    {
        get { return  _currentEventData; }
        private set { _currentEventData = value; }
    }

    // Delegate
    [SerializeField]
    private OnBeginEventEvent _onBeginEvent;
    public  OnBeginEventEvent  onBeginEvent
    {
        get { return  _onBeginEvent; }
        private set { _onBeginEvent = value; }
    }

    [SerializeField]
    private OnEndEventEvent _onEndEvent;
    public  OnEndEventEvent  onEndEvent
    {
        get { return  _onEndEvent; }
        private set { _onEndEvent = value; }
    }

    void Awake()
    {
        eventManager = this;

        onEndEvent.AddListener(MSGameInstance.Get().AddEventCount);
    }

    public void BeginEvent(EventData eventData)
    {
        currentEventData = eventData;
        onBeginEvent.Invoke(currentEventData);
    }
    public void EndEvent()
    {
        onEndEvent.Invoke(currentEventData);
        currentEventData = null;
    }

    public bool PlayDreampostVictoryEvent()
    {
        var events = MSDataManager.Get().globalInitializeData.globalDreampostVictoryEvents;
        var eventData = PlayEventFromList(events);

        if (eventData == null)
            return false;
        
        BeginEvent(eventData);
        onEndEvent.AddListener(OnDreampostVictoryEventEnded);
        return true;
    }
    public bool PlayDreampostDefeatEvent()
    {
        var events = MSDataManager.Get().globalInitializeData.globalDreampostDepeatEvents;
        var eventData = PlayEventFromList(events);

        if (eventData == null)
            return false;
        
        BeginEvent(eventData);
        onEndEvent.AddListener(OnDreampostDefeatEventEnded);
        return true;
    }
    private EventData PlayEventFromList(List<EventData> events)
    {
        var availableEvents = new List<EventData>();
        foreach (var eventData in events)
        {
            if (MSGameInstance.Get().IsEventAvailable(eventData))
                availableEvents.Add(eventData);
        }

        availableEvents = availableEvents.OrderBy(x => x.priority).ToList();
        if (availableEvents.Count > 0) 
            return availableEvents[0];
        
        return null;
    }

    private void OnDreampostVictoryEventEnded(EventData eventData)
    {
        var layoutDreamflow = UGUIManager.Get().GetLayout(LayoutType.Dreamflow) as LayoutDreamflow;
        layoutDreamflow.SetRewardPopUp();
        
        onEndEvent.RemoveListener(OnDreampostVictoryEventEnded);
    }
    private void OnDreampostDefeatEventEnded(EventData eventData)
    {
        UGUIManager.Get().OpenLayout(LayoutType.Main);
        UGUIManager.Get().SetLayoutMainMode(LayoutMainMode.Schedule);
        
        onEndEvent.RemoveListener(OnDreampostDefeatEventEnded);
    }
}
