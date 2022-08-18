using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class PopUpWorldMapSpotDetail : PopUpBase
{
    public Button buttonReturn;

    public Image detailBackgroundImage;
    public List<Button> detailEventButtons;
    public Button selectedEventButtons;


    public override void SetManager(UGUIManager manager)
    {
        base.SetManager(manager);
        buttonReturn.GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);

        for (int i = 0; i < detailEventButtons.Count; i++)
        {
            detailEventButtons[i].GetComponent<ElementBase>().SetManager(manager);
            detailEventButtons[i].GetComponent<ElementBase>().SetParentComponent(this);
        }
    }

    public void SetSpotDetail(SpotData targetSpotData, Dictionary<FriendshipType, WeekendEventData> options)
    {
        detailBackgroundImage.sprite = targetSpotData.backgroundSprite;

        foreach (Button button in detailEventButtons)
        {
            button.gameObject.SetActive(false);
            button.interactable = false;
        }

        int index = 0;
        foreach (KeyValuePair<FriendshipType, WeekendEventData> eventData in options)
        {
            if (index >= detailEventButtons.Count)
                continue;

            if (eventData.Value.option.spotType != targetSpotData.type)
                continue;

            detailEventButtons[index].gameObject.SetActive(true);
            detailEventButtons[index].interactable = true;

            detailEventButtons[index].GetComponent<ElementFreeTimeEventButton>().eventData = eventData.Value;
            detailEventButtons[index].GetComponent<Image>().sprite = eventData.Value.option.sprite;
            detailEventButtons[index].GetComponent<RectTransform>().anchoredPosition = eventData.Value.option.position;

            index++;
        }
    }

    public void CheckNoAvailableEvent()
    {
        foreach (Button button in detailEventButtons)
        {
            if (button.interactable == true)
                return;
        }

        ClosePopUp();
        StartCoroutine(CloseWorldMapLayout());
    }

    void OnButtonReturnClicked()
    {
        ClosePopUp();
        StartCoroutine(CloseWorldMapLayout());
    }

    IEnumerator CloseWorldMapLayout()
    {
        yield return new WaitForSeconds(0.3f);
        manager.CloseLayout(LayoutType.WorldMap);

        manager.OpenLayout(LayoutType.Main);
        manager.SetLayoutMainMode(LayoutMainMode.WeekendNight);
    }
}
