using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamBattleFigureStatusBuff : MonoBehaviour
{
    [SerializeField] Image iconBuff;
    [SerializeField] Text  textTimer;

    BuffInstance instance;

    public BuffInstance GetBuffInstance() { return instance; }

    public void SetBuffInstance(BuffInstance buffInstance)
    {
        if (instance != null)
        {
            instance.onBuffTimerChanged.RemoveListener(OnTimerChanged);
        }

        instance = buffInstance;
        if (instance == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            instance.onBuffTimerChanged.AddListener(OnTimerChanged);

            iconBuff.sprite = instance.data.iconStatusMini;
            textTimer. text = instance.timer.ToString();
        }
    }
    public void OnTimerChanged(int value)
    {
        textTimer.text = value.ToString();
    }
}
