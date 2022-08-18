using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class PopUpSettings : PopUpBase
{
    [SerializeField] Button buttonReturn;
    
    [SerializeField] Button buttonMusicVolumeUp;
    [SerializeField] Button buttonMusicVolumeDown;
    [SerializeField] List<Image> imageMusicGauges;
    
    [SerializeField] Button buttonSoundVolumeUp;
    [SerializeField] Button buttonSoundVolumeDown;
    [SerializeField] List<Image> imageSoundGauges;
    
    [SerializeField] Button buttonLanguagePrevious;
    [SerializeField] Button buttonLanguageNext;
    [SerializeField] LocalizeStringEvent textLanguage;
    
    [SerializeField] Button buttonGooglePlay;
    [SerializeField] Button buttonTwitter;
    [SerializeField] Button buttonReturnToTitle;

    [SerializeField] Sprite spriteGaugeOn;
    [SerializeField] Sprite spriteGaugeOff;

    List<LocaleIdentifier> supportLocaleIdentifiers;
    
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);
        
        
        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        
        buttonMusicVolumeUp.onClick.AddListener(OnButtonMusicVolumeUpClicked);
        buttonMusicVolumeDown.onClick.AddListener(OnButtonMusicVolumeDownClicked);
        
        buttonSoundVolumeUp.onClick.AddListener(OnButtonSoundVolumeUpClicked);
        buttonSoundVolumeDown.onClick.AddListener(OnButtonSoundVolumeDownClicked);
        
        buttonLanguagePrevious.onClick.AddListener(OnButtonLanguagePreviousClicked);
        buttonLanguageNext.onClick.AddListener(OnButtonLanguageNextClicked);
        
        buttonGooglePlay.onClick.AddListener(OnButtonGooglePlayClicked);
        buttonTwitter.onClick.AddListener(OnButtonTwitterClicked);
        buttonReturnToTitle.onClick.AddListener(OnButtonReturnToTitleClicked);

        // Sound and Music
        for (var i = 0; i < 4; i++)
        {
            imageMusicGauges[i].sprite = i >= MSGameInstance.Get().preferences.volumeMusic ? spriteGaugeOff : spriteGaugeOn;
            imageSoundGauges[i].sprite = i >= MSGameInstance.Get().preferences.volumeSound ? spriteGaugeOff : spriteGaugeOn;
        }
        
        // Locale
        supportLocaleIdentifiers = new List<LocaleIdentifier>();
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            supportLocaleIdentifiers.Add(locale.Identifier);
        }
    }
    
    void OnButtonReturnClicked()
    {
        ClosePopUp();
    }
    
    void OnButtonMusicVolumeUpClicked()
    {
        var volume = MSGameInstance.Get().preferences.volumeMusic;
        if (MSGameInstance.Get().preferences.volumeMusic < 5)
            volume++;
        
        MSGameInstance.Get().SetMusicVolume(volume);
        imageMusicGauges[volume - 1].sprite = spriteGaugeOn;
    }
    void OnButtonMusicVolumeDownClicked()
    {
        var volume = MSGameInstance.Get().preferences.volumeMusic;
        if (MSGameInstance.Get().preferences.volumeMusic > 0)
            volume--;
        
        MSGameInstance.Get().SetMusicVolume(volume);
        imageMusicGauges[volume].sprite = spriteGaugeOff;
    }
    
    void OnButtonSoundVolumeUpClicked()
    {
        var volume = MSGameInstance.Get().preferences.volumeSound;
        if (MSGameInstance.Get().preferences.volumeSound < 5)
            volume++;
        
        MSGameInstance.Get().SetSoundVolume(volume);
        imageSoundGauges[volume - 1].sprite = spriteGaugeOn;
    }
    void OnButtonSoundVolumeDownClicked()
    {
        var volume = MSGameInstance.Get().preferences.volumeSound;
        if (MSGameInstance.Get().preferences.volumeSound > 0)
            volume--;
        
        MSGameInstance.Get().SetSoundVolume(volume);
        imageSoundGauges[volume].sprite = spriteGaugeOff;
    }
    
    void OnButtonLanguagePreviousClicked()
    {
        var identifier = MSGameInstance.Get().preferences.localeIdentifier;
        var index = supportLocaleIdentifiers.IndexOf(identifier);
        index--;

        if (index < 0)
            index = supportLocaleIdentifiers.Count - 1;

        SelectLocale(index);
    }
    void OnButtonLanguageNextClicked()
    {
        var identifier = MSGameInstance.Get().preferences.localeIdentifier;
        var index = supportLocaleIdentifiers.IndexOf(identifier);
        index++;

        if (index == supportLocaleIdentifiers.Count)
            index = 0;

        SelectLocale(index);
    }
    void SelectLocale(int index)
    {
        var identifier = supportLocaleIdentifiers[index];
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (identifier == locale.Identifier)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        if (identifier == "ko-KR")
            textLanguage.SetEntry("LanguageKorean");
        else
            textLanguage.SetEntry("LanguageEnglish");
        
        MSGameInstance.Get().preferences.localeIdentifier = identifier;
    }
    
    void OnButtonGooglePlayClicked()
    {
        
    }
    void OnButtonTwitterClicked()
    {
        
    }
    void OnButtonReturnToTitleClicked()
    {
        UGUIManager.Get().OpenPopUp(PopUpType.ReturnToTitleMessage);
    }
}
