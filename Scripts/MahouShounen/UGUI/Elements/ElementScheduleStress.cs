using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementScheduleStress : ElementBase
{
    public int  value;
    public Text textValue;

    public void SetValue(int value)
    {
        this.value = value;
        textValue.text = (value > 0) ? "+" + value.ToString() : 
                                             value.ToString();
    }
}
