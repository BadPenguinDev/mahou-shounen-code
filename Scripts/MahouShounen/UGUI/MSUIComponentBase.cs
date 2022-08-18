using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSUIComponentBase : MonoBehaviour
{
    protected UGUIManager manager;
    public virtual void SetManager(UGUIManager uiManager)
    {
        if (manager == uiManager)
            return;
        
        manager = uiManager;
        
        foreach (var button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.RemoveListener(uiManager.PlayAudioClipButton);
            button.onClick.AddListener(uiManager.PlayAudioClipButton);
        }
        foreach (var toggle in GetComponentsInChildren<Toggle>(true))
        {
            toggle.onValueChanged.RemoveListener(uiManager.PlayAudioClipToggle);
            toggle.onValueChanged.AddListener(uiManager.PlayAudioClipToggle);
        }
    }
    public virtual UGUIManager GetManager() { return manager; }

    public virtual void OnDialogueFinished()
    {

    }
}
