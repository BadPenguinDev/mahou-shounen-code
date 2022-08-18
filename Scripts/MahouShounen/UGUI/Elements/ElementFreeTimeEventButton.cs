using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementFreeTimeEventButton : ElementBase
{
    public EventData eventData;

    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);

        MSEventManager.Get().onEndEvent.AddListener(OnEndEvent);
        GetComponent<Button>().onClick. AddListener(OnFreeTimeEventButtonClicked);
    }

    public void OnFreeTimeEventButtonClicked()
    {
        var popUp = parentComponent as PopUpWorldMapSpotDetail;
        if (!popUp.isActiveAndEnabled)
            return;
        
        MSEventManager.Get().BeginEvent(eventData);

        popUp.selectedEventButtons = GetComponent<Button>();
    }
    public void OnEndEvent(EventData eventData)
    {
        var popUp = parentComponent as PopUpWorldMapSpotDetail;
        if (!popUp.isActiveAndEnabled)
            return;
        
        if (this.eventData == eventData)
            GetComponent<Button>().interactable = false;

        popUp.CheckNoAvailableEvent();
    }
}
