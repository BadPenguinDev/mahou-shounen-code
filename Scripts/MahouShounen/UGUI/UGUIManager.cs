using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MSUGUIPalette { Main, Sub, Positive, Negative, Disabled, Pointed, Red, Orange, Yellow, Green, Lime, Teal, Cyan, Blue, Navy, Purple, Magenta, Pink }

public class UGUIManager : MonoBehaviour
{
    public static UGUIManager uiManager;

    public List<GameObject> layoutPrefabs;
    public List<GameObject>  popUpPrefabs;
    public RectTransform windowsTransform;

    [Header("UGUIManager")]
    public DialogueController dialogueController;
    public Image imageBackground;

    public  Dictionary<LayoutType, LayoutBase> layouts;
    public  Dictionary< PopUpType,  PopUpBase>  popUps;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   audioClipButton;

    private IEnumerator playAudioClipButtonCoroutine;
    
    private List<MSUIComponentBase> activeUIComponents;


    // Access
    public static UGUIManager Get()
    {
        return uiManager;
    }

    void Awake()
    {
        uiManager = this;
    }
    void Start()
    {
        activeUIComponents = new List<MSUIComponentBase>();

        layouts = new Dictionary<LayoutType, LayoutBase>();
        foreach (var prefab in layoutPrefabs)
        {
            var layoutObject = Instantiate(prefab, windowsTransform);
            var layout = layoutObject.GetComponent<LayoutBase>();
            layout.SetManager(this);
            
            layouts.Add(layout.type, layout);
        }

        popUps = new Dictionary<PopUpType, PopUpBase>();
        foreach (var prefab in popUpPrefabs)
        {
            var layoutObject = Instantiate(prefab, windowsTransform);
            var popUp = layoutObject.GetComponent<PopUpBase>();
            popUp.SetManager(this);
            
            popUps.Add(popUp.type, popUp);
        }

        MSEventManager.Get().onBeginEvent.AddListener(OnBeginEvent);

        foreach (var pair in layouts)
        {
            pair.Value.gameObject.SetActive(false);
        }
        foreach (var pair in  popUps)
        {
            pair.Value.gameObject.SetActive(false);
        }
    }

    public LayoutBase GetLayout(LayoutType type)
    {
        return layouts[type];
    }
    public  PopUpBase GetPopUp  (PopUpType type)
    {
        return popUps[type];
    }

    // Layout
    public LayoutBase   OpenLayout(LayoutType type, string name = "On")
    {
        StartCoroutine(StartLayoutDelay(type, name));
        activeUIComponents.Add(layouts[type]);

        return layouts[type];
    }
    public void        CloseLayout(LayoutType type, string name = "Off")
    {
        layouts[type].CloseLayout(name);
        activeUIComponents.Remove(layouts[type]);
    }
    public IEnumerator StartLayoutDelay(LayoutType type, string name = "On")
    {
        yield return new WaitForSeconds(layouts[type].openDelay);
        layouts[type].OpenLayout(name);
    }

    // Pop Up
    public PopUpBase OpenPopUp(PopUpType type)
    {
        popUps[type].OpenPopUp();
        activeUIComponents.Add(popUps[type]);

        return popUps[type];
    }
    public void     ClosePopUp(PopUpType type)
    {
        popUps[type].ClosePopUp();
        activeUIComponents.Remove(popUps[type]);
    }

    // Dialogue
    public void OnBeginEvent(EventData eventData)
    {
        dialogueController.SetTargetDialogueContainer(eventData.dialogueContainer);

        dialogueController.layoutDialogue.gameObject.SetActive(true);
        dialogueController.layoutDialogue.GetComponent<Animator>().Play("On");
    }
    public void OnDialogueFinished()
    {
        MSEventManager.Get().EndEvent();

        dialogueController.FinishDialogue();
        SetDefaultLayoutActive();
        activeUIComponents[activeUIComponents.Count - 1].OnDialogueFinished();
    }

    // Method: Main
    public void SetLayoutMainMode(LayoutMainMode mode)
    {
        layouts[LayoutType.Main].GetComponent<LayoutMain>().SetLayoutMainMode(mode);
    }
    public void SetDefaultLayoutActive()
    {
        if (activeUIComponents.Count != 0) 
            return;
        
        layouts[LayoutType.Main].OpenLayout();
        activeUIComponents.Add(layouts[LayoutType.Main]);

        SetLayoutMainMode(MSGameInstance.Get().layoutMainMode);
    }

    // Background
    public void SetBackgroundNight()
    {
        StartCoroutine(FadeBackgroundAlpha(1.0f, 0.5f));
    }
    public void SetBackgroundDay()
    {
        StartCoroutine(FadeBackgroundAlpha(0.0f, 1.0f));
    }
    IEnumerator FadeBackgroundAlpha(float value, float time)
    {
        var alpha = imageBackground.color.a;
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            var newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, value, t));
            imageBackground.color = newColor;
            yield return null;
        }
    }
    
    // Sound
    public void PlayAudioClip(AudioClip clip)
    {
        if (playAudioClipButtonCoroutine != null)
            StopCoroutine(playAudioClipButtonCoroutine);

        playAudioClipButtonCoroutine = PlayAudioClipCoroutine(clip);
        StartCoroutine(playAudioClipButtonCoroutine);
    }
    public void PlayAudioClipButton()
    {
        if (playAudioClipButtonCoroutine != null)
            StopCoroutine(playAudioClipButtonCoroutine);

        playAudioClipButtonCoroutine = PlayAudioClipCoroutine(audioClipButton);
        StartCoroutine(playAudioClipButtonCoroutine);
    }
    public void PlayAudioClipToggle(bool status)
    {
        if (playAudioClipButtonCoroutine != null)
            StopCoroutine(playAudioClipButtonCoroutine);

        playAudioClipButtonCoroutine = PlayAudioClipCoroutine(audioClipButton);
        StartCoroutine(playAudioClipButtonCoroutine);
    }
    public void StopPlayAudioToggle()
    {
        if (playAudioClipButtonCoroutine != null)
            StopCoroutine(playAudioClipButtonCoroutine);
    }

    IEnumerator PlayAudioClipCoroutine(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, 0.2f * MSGameInstance.Get().preferences.volumeSound);
        for (var t = 0f; t < 1.0f; t += Time.deltaTime / 0.1f)
        {
            yield return null;
        }
    }
        

    // Palette
    public UIPaletteData GetPaletteData(MSUGUIPalette type)
    {
        return MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == type);
    }
    public UIPaletteData GetSchedulePaletteData(ScheduleType type, bool isCheckCurrency = false)
    {
        var schedulePalette = MSDataManager.Get().globalPaletteData.schedulePalettes.Find(x => x.type == type).palette;
        
        if (isCheckCurrency &&
            type == ScheduleType.Class &&
            MSGameInstance.Get().currency <= 0)
            schedulePalette = MSUGUIPalette.Disabled;
        
        return MSDataManager.Get().globalPaletteData.paletteDatas.Find(x => x.type == schedulePalette);
    }

    public void SetScrollBarTextPreset(Text targetText, float size)
    {
        var targetPreset = MSDataManager.Get().globalPaletteData.scrollBarTextPresets[0];
        foreach (var preset in MSDataManager.Get().globalPaletteData.scrollBarTextPresets)
        {
            if (size < preset.size)
                break;
            else
                targetPreset = preset;
        }
        
        var textTransform = targetText.GetComponent<RectTransform>();
        textTransform.anchorMin = targetPreset.anchorMin;
        textTransform.anchorMax = targetPreset.anchorMax;
        textTransform.pivot = targetPreset.pivot;
        textTransform.anchoredPosition = targetPreset.position;
        targetText.alignment = targetPreset.alignment;
    }
}

