using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MSTitleController : MonoBehaviour
{
    public Animator ShadowAnimator;
    public Animator PopUpForceNewGameAnimator;

    public Button buttonLoad;
    public OutlineColored outlineTextLoad;
    
    public UIPaletteData   activePalette;
    public UIPaletteData deactivePalette;
    
    public AudioSource source;
    public AudioClip   clipButton;
    
    private IEnumerator playButtonSoundCoroutine;
    
    void Start()
    {
        ShadowAnimator.enabled = true;

        // Set Game Save Active
        var path = Path.Combine(Application.persistentDataPath, "GameSave.json");
        if (!File.Exists(path))
        {
            buttonLoad.interactable = false;
            
            buttonLoad.image.sprite = deactivePalette.buttonStandaloneSprite;
            outlineTextLoad.effectColor = deactivePalette.outlineColor;
        }
        else
        {
            buttonLoad.interactable = true;
            
            buttonLoad.image.sprite = activePalette.buttonStandaloneSprite;
            outlineTextLoad.effectColor = activePalette.outlineColor;
        }
        
        source.volume = MSGameInstance.Get().preferences.volumeMusic * 0.2f;
    }

    public void StartNewGame()
    {
        var path = Path.Combine(Application.persistentDataPath, "GameSave.json");
        if (!File.Exists(path))
            StartCoroutine(nameof(StartNewGameCoroutine));
        else
        {
            PopUpForceNewGameAnimator.gameObject.SetActive(true);
            PopUpForceNewGameAnimator.Play("On");
        }
    }
    public void ForceStartNewGame()
    {
        StartCoroutine(nameof(StartNewGameCoroutine));
    }
    public void LoadGame()
    {
        StartCoroutine(nameof(LoadGameCoroutine));
    }
    
    IEnumerator StartNewGameCoroutine()
    {
        ShadowAnimator.Play("ShadowOn");
        for (float t = 0; t < 1.0f; t += Time.deltaTime / 1.5f)
        {
            source.volume = MSGameInstance.Get().preferences.volumeMusic * 0.2f * (1f - t);
            yield return null;
        }
        
        SceneManager.LoadScene("NewGameScene", LoadSceneMode.Single);
    }
    IEnumerator LoadGameCoroutine()
    {
        ShadowAnimator.Play("ShadowOn");
        for (float t = 0; t < 1.0f; t += Time.deltaTime / 1.5f)
        {
            source.volume = MSGameInstance.Get().preferences.volumeMusic * 0.2f * (1f - t);
            yield return null;
        }
        
        MSGameInstance.Get().LoadGameData();
        SceneManager.LoadScene("InGameScene", LoadSceneMode.Single);
    }
    
    
    public void StartPlayButtonSound()
    {
        if (playButtonSoundCoroutine != null)
            StopCoroutine(playButtonSoundCoroutine);

        playButtonSoundCoroutine = PlayButtonSoundCoroutine();
        StartCoroutine(playButtonSoundCoroutine);
    }
    IEnumerator PlayButtonSoundCoroutine()
    {
        source.PlayOneShot(clipButton, 0.2f * MSGameInstance.Get().preferences.volumeSound);
        for (var t = 0f; t < 1.0f; t += Time.deltaTime / 0.1f)
        {
            yield return null;
        }
    }
}
