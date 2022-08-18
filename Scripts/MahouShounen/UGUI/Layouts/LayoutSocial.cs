using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class LayoutSocial : LayoutBase
{
    public CharacterData characterData;
    public GameObject buttonReturn;

    [Header("Social Info")]
    public Text textName;
    public Text textBirthday;
    public Scrollbar gaugeFriendship;

    [Header("Social Portrait")]
    public Image portraitCharacter;
    public Image clothingCharacter;

    [Header("Social Schedule")]
    public List<ElementSocialSchedule> elementSocialSchedules;
    public ElementSocialSpot elementSocialSpot;

    [Header("Social Characters")]
    public Transform fieldSocialCharacterList;
    public List<ElementSocialCharacterButton> elementSocialCharacterButtons;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        buttonReturn.GetComponent<Button>().onClick.AddListener(OnButtonReturnClicked);

        for (int i = 0; i < elementSocialCharacterButtons.Count; i++)
        {
            ElementSocialCharacterButton element = elementSocialCharacterButtons[i];
            element.button.onClick.AddListener(delegate { SetCharacterType(element); });
        }
    }

    public override void   OpenLayout(string layoutName)
    {
        base.OpenLayout(layoutName);
        SetCharacterType(elementSocialCharacterButtons[0]);
    }
    public override void UpdateLayout()
    {
        base.UpdateLayout();
    }

    public void SetCharacterType(ElementSocialCharacterButton elementSocialCharacterButton)
    {
        CharacterData data = MSDataManager.Get().characterDatas.Find(x => x.friendshipType == elementSocialCharacterButton.friendshipType);
        if (data == null)
            return;

        characterData = data;

        // Social Info
        textName.GetComponent<LocalizeStringEvent>().SetEntry(characterData.friendshipType.ToString());
        textName.GetComponent<LocalizeStringEvent>().SetTable("Characters");

        string birthdayString = LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", characterData.day.ToString())  + ", "
                              + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", characterData.week.ToString()) + ", "
                              + LocalizationSettings.StringDatabase.GetLocalizedString("DateTimes", characterData.month.ToString());

        textBirthday.text = birthdayString;

        int frienshipValue = 0;
        MSGameInstance.Get().friendships.TryGetValue(characterData.friendshipType, out frienshipValue);
        gaugeFriendship.value = frienshipValue / MSCommon.MaxFriendshipGauge;

        // Social Portrait
        portraitCharacter.sprite = characterData.emotionSprites[0];
        clothingCharacter.sprite = characterData.costumeSprites[0];

        // Social Schedule
        foreach (CharacterSchedule schedule in characterData.characterSchedules)
        {
            ElementSocialSchedule element = elementSocialSchedules.Find(x => x.day == schedule.day);
            if (element == null)
                continue;

            element.SetSchedule(schedule.schedule);
        }

        // Social Spot
        WeekendEventData eventData = MSGameInstance.Get().FindCurrentWeekendEvent(characterData);
        elementSocialSpot.SetWeekendEvent(eventData);
    }


    void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
}
