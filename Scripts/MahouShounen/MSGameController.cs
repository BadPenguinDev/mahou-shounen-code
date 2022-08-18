using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSGameController : MonoBehaviour
{
    public Animator shadowAnimator;
    public Animator transitionAnimator;

    public AudioSource audioSourceDay;
    public AudioSource audioSourceNight;
    public AudioSource audioSourceEvent;

    bool isInitialized = false;

    AudioSource   prevAudioSource;
    AudioSource targetAudioSource;
    IEnumerator audioSourceTransitionCoroutine;

    const float audioTransitionTimer = 2f;

        
    void Awake()
    {
        Application.targetFrameRate = 30;
        
        audioSourceDay.  Stop();
        audioSourceNight.Stop();
        audioSourceEvent.Stop();

        MSGameInstance.Get().onMusicVolumeChanged.AddListener(SetMusicVolume);
        SetMusicVolume(MSGameInstance.Get().preferences.volumeMusic);
    }

    void Start()
    {
        shadowAnimator.gameObject.SetActive(true);

        if (MSDataManager.Get() == null)
            Debug.LogError("Can not load Data Manager!");
    }

    private void Update()
    {
        if (isInitialized == false)
        {
            if (MSGameInstance.Get().isNewGame == true)
            {
                var eventData = MSDataManager.Get().globalInitializeData.introEvent;
                MSEventManager.Get().BeginEvent(eventData);

                UGUIManager.Get().dialogueController.layoutDialogue.gameObject.SetActive(true);
                UGUIManager.Get().GetLayout(LayoutType.Main).gameObject.SetActive(false);
                UGUIManager.Get().SetLayoutMainMode(LayoutMainMode.Schedule);
            }
            else
            {
                UGUIManager.Get().SetDefaultLayoutActive();
            }

            isInitialized = true;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
            DEBUGADDINTEL();
    }

    public void PlayTransition()
    {
        transitionAnimator.gameObject.SetActive(true);
        transitionAnimator.Play("Transition");
    }
    public void ChangeTransition()
    {
        UGUIManager.Get().OnDialogueFinished();
    }
    public void FinishTransition()
    {
        transitionAnimator.gameObject.SetActive(false);
    }

    public void SetMusicVolume(int volume)
    {
        audioSourceDay.  volume = volume * 0.2f;
        audioSourceNight.volume = volume * 0.2f;
        audioSourceEvent.volume = volume * 0.2f;
    }
    
    // Music
    public void PlayAudioByLayoutMainMode(LayoutMainMode mode)
    {
        var audioIn = audioSourceDay;
        if (mode == LayoutMainMode.WeekendNight)
            audioIn = audioSourceNight;

        if (audioIn == targetAudioSource)
            return;
        
        if (audioSourceTransitionCoroutine != null)
            StopCoroutine(audioSourceTransitionCoroutine);
        
        audioSourceTransitionCoroutine = AudioSourceTransitionCoroutine(audioIn, targetAudioSource);
        StartCoroutine(audioSourceTransitionCoroutine);
    }
    public void PlayEventMusic(AudioClip clip)
    {
        if (audioSourceTransitionCoroutine != null)
            StopCoroutine(audioSourceTransitionCoroutine);

        prevAudioSource = targetAudioSource;
        audioSourceEvent.clip = clip;
        audioSourceTransitionCoroutine = AudioSourceTransitionCoroutine(audioSourceEvent, targetAudioSource);
        StartCoroutine(audioSourceTransitionCoroutine);
    }
    public void StopEventMusic()
    {
        if (audioSourceTransitionCoroutine != null)
            StopCoroutine(audioSourceTransitionCoroutine);

        audioSourceTransitionCoroutine = AudioSourceTransitionCoroutine(prevAudioSource, targetAudioSource);
        StartCoroutine(audioSourceTransitionCoroutine);
    }
    
    IEnumerator AudioSourceTransitionCoroutine(AudioSource audioIn, AudioSource audioOut)
    {
        var isAudioInNotNull  = audioIn  != null;
        var isAudioOutNotNull = audioOut != null;
        var volume = MSGameInstance.Get().preferences.volumeMusic * 0.2f;
        
        audioIn.Play();
        
        for (var t = 0f; t < 1f; t += Time.deltaTime / audioTransitionTimer)
        {
            if (isAudioInNotNull)
                audioIn. volume = t * volume;
            
            if (isAudioOutNotNull)
                audioOut.volume = (1f - t) * volume;

            yield return null;
        }
        
        if (isAudioOutNotNull)
            audioOut.Stop();
        
        targetAudioSource = audioIn;
    }


    public void DEBUGADDINTEL()
    {
        MSGameInstance.Get().playerStats[Stat.Intelligence].value += 32;
        MSGameInstance.Get().onStatChanged.Invoke(Stat.Intelligence, MSGameInstance.Get().playerStats[Stat.Intelligence]);
        Debug.Log("Current Intel = " + MSGameInstance.Get().playerStats[Stat.Intelligence].value.ToString());
    }
}
