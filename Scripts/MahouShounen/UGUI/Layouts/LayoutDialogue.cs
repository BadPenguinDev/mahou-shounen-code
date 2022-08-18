using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutDialogue : LayoutBase
{
    public Animator dialogueAnimator;

    public void SetDialogueAnimatorDisable()
    {
        dialogueAnimator.enabled = false;
    }
}
