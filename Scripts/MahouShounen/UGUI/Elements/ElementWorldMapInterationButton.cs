using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementWorldMapInterationButton : ElementBase
{
    public Spot spot;

    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);
        GetComponent<Image>(). alphaHitTestMinimumThreshold = 1.0f;
        GetComponent<Button>().onClick.AddListener(OnButtonWorldMapSpotClicked);
    }

    void OnButtonWorldMapSpotClicked()
    {
        PopUpWorldMapSpot popUp = manager.OpenPopUp(PopUpType.WorldMapSpot) as PopUpWorldMapSpot;
        popUp.SetTargetSpot(spot);
    }
}
