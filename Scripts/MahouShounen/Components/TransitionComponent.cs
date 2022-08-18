using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransitionComponent : MonoBehaviour
{
    public DialogueController DialogueController;

    public void TriggerTransition()
    {
        if (DialogueController != null)
            DialogueController.OnTransitionTriggered();
    }

    void SetAnimatorSpeed(float speed)
    {
        GetComponent<Animator>().speed = speed;
    }
}
