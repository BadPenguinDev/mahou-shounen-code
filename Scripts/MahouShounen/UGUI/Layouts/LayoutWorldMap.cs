using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class LayoutWorldMap : LayoutBase
{
    public GameObject buttonReturn;

    [Header("Layout World Map")]
    public List<ElementWorldMapInterationButton> worldMapInterationButtons;

    // Pop Up World Map Spot
    public GameObject popUpWorldMapSpot;

    public Text textTitle;
    public Image spotImage;
    public List<Image> spotCharacterImages;

    // Pop Up World Map Spot Detail
    public GameObject popUpWorldMapSpotDetail;

    public Image detailBackgroundImage;
    public List<Button> detailEventButtons;
    public Button selectedEventButtons;


    public SpotData targetSpotData;
    public Dictionary<FriendshipType, WeekendEventData> options;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        buttonReturn.GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);

        for (int i = 0; i< worldMapInterationButtons.Count; i++)
        {
            worldMapInterationButtons[i].SetManager(uiManager);
            worldMapInterationButtons[i].SetParentComponent(this);
        }
    }

    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }

}

