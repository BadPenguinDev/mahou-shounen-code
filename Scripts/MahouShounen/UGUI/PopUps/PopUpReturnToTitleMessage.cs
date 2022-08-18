using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopUpReturnToTitleMessage : PopUpBase
{
    public Button buttonReturnToTitle;
    public Button buttonContinue;

    public Image foregroundShadow;

    public override void SetManager(UGUIManager manager)
    {
        base.SetManager(manager);

        buttonReturnToTitle.onClick.AddListener(OnButtonReturnToTitle);
        buttonContinue.onClick.AddListener(OnButtonContinue);
    }

    void OnButtonReturnToTitle()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    void OnButtonContinue()
    {
        ClosePopUp();
    }

    IEnumerator FadeOutCoroutine()
    {
        foregroundShadow.gameObject.SetActive(true);
        
        for (var t = 0f; t < 1f; t += Time.deltaTime / 1f)
        {
            foregroundShadow.color = new Color(0f, 0f, 0f, t);
            yield return null;
        }
        foregroundShadow.color = new Color(0f, 0f, 0f, 1f);
        
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
