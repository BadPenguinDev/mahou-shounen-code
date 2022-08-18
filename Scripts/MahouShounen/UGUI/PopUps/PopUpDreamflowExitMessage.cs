using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDreamflowExitMessage : PopUpBase
{
    public Button buttonStopDreamflow;
    public Button buttonContinue;

    public override void SetManager(UGUIManager manager)
    {
        base.SetManager(manager);

        buttonStopDreamflow.onClick.AddListener(OnButtonStopDreamflow);
        buttonContinue.onClick.AddListener(OnButtonContinue);
    }

    void OnButtonStopDreamflow()
    {
        ClosePopUp();
        manager.CloseLayout(LayoutType.Dreamflow);

        manager.OpenLayout(LayoutType.Main);
        manager.SetLayoutMainMode(LayoutMainMode.Schedule);
    }
    void OnButtonContinue()
    {
        ClosePopUp();
    }
}
