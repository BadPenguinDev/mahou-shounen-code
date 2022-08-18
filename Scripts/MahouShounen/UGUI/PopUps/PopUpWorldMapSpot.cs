using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class PopUpWorldMapSpot : PopUpBase
{
    public Button buttonReturn;
    public Button buttonVisitSpot;

    public Text textTitle;
    public Image spotImage;
    public List<Image> spotCharacterImages;

    SpotData spotData;
    Dictionary<FriendshipType, WeekendEventData> options;


    public override void SetManager(UGUIManager manager)
    {
        base.SetManager(manager);
        buttonReturn.   GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);
        buttonVisitSpot.GetComponent<Button>().onClick.AddListener(OnButtonVisitSpotClicked);

        options = new Dictionary<FriendshipType, WeekendEventData>();
    }

    public void SetTargetSpot(Spot spot)
    {
        spotData = MSDataManager.Get().spotDatas.Find(x => x.type == spot);

        spotImage.sprite = spotData.spotSprite;
        textTitle.GetComponent<LocalizeStringEvent>().SetEntry(spot.ToString());

        foreach (Image image in spotCharacterImages)
        {
            image.gameObject.SetActive(false);
        }

        options = MSGameInstance.Get().GetEventOptionsByCondition(spotData.type);

        int index = 0;
        foreach (KeyValuePair<FriendshipType, WeekendEventData> eventData in options)
        {
            if (index >= spotCharacterImages.Count)
                continue;

            if (eventData.Value.option.spotType != spotData.type)
                continue;

            CharacterData charData = MSDataManager.Get().characterDatas.Find(x => x.friendshipType == eventData.Key);

            spotCharacterImages[index].sprite = charData.iconSprite;
            spotCharacterImages[index].gameObject.SetActive(true);

            index++;
        }
    }

    void OnButtonReturnClicked()
    {
        ClosePopUp();
    }
    void OnButtonVisitSpotClicked()
    {
        PopUpWorldMapSpotDetail popUp = manager.OpenPopUp(PopUpType.WorldMapSpotDetail) as PopUpWorldMapSpotDetail;
        popUp.SetSpotDetail(spotData, options);

        ClosePopUp();
    }
}
