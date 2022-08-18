using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceToStartScene : MonoBehaviour
{    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstLoad()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "TitleScene", StringComparison.Ordinal) != 0)
        {
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

    }
}
