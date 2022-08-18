using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCGComponent : MonoBehaviour
{
    public DialogueController DialogueController;

    void FinishAnimation()
    {
        if (DialogueController != null)
            DialogueController.FinishAnimation();
    }

    void SetAnimatorSpeed(float speed)
    {
        GetComponent<Animator>().speed = speed;
    }
}
