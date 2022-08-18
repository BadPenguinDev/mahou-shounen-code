using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTransitionComponent : MonoBehaviour
{
    public MSGameController GameController;

    void ChangeTransition()
    {
        if (GameController != null)
            GameController.ChangeTransition();
    }
    void FinishTransition()
    {
        if (GameController != null)
            GameController.FinishTransition();
    }
}
