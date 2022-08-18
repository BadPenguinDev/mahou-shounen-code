using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CasualwearType
{
    SchoolUniform, Butler, Admiral
}
public enum CostumeType
{
    MagicalGirl, MaidDress, Sailor
}
public enum PortraitType
{
    None,
    Shounen,  ShounenMagical,
    Familiar, FamiliarHuman, 
    Scientist, Enterprise, ArcheryPlayer, Violinist, Chef, Cartoonist, Programmer, Idol, Prince, Ballerino
};
public enum PortraitOrientation { Left, Right };

public enum DialogueDecoratorType { Speech, FrameCG, FullCG, AnimationCG, Transition, PlaySFX, PlayMusic, CameraShake };

public interface DialogueDecoratorData
{

}

[System.Serializable]
public struct DialogueDecoratorSpeechData : DialogueDecoratorData
{
    public PortraitType PortraitType;
    public EmotionType EmotionType;
    public PortraitOrientation PortraitOrientation;
}

[System.Serializable]
public struct DialogueDecoratorFrameCGData : DialogueDecoratorData
{
    public Sprite SpriteFrameCG;
}

[System.Serializable]
public struct DialogueDecoratorFullCGData : DialogueDecoratorData
{
    public Sprite SpriteFullCG;
    public bool isIgnoreDialogue;
}

[System.Serializable]
public struct DialogueDecoratorAnimationCGData : DialogueDecoratorData
{
    public GameObject AnimationCG;
    public bool isIgnoreDialogue;
}

[System.Serializable]
public struct DialogueDecoratorTransitionData : DialogueDecoratorData
{
    public GameObject Transition;
}

[System.Serializable]
public struct DialogueDecoratorPlaySFXData : DialogueDecoratorData
{
    public AudioClip clip;
}

[System.Serializable]
public struct DialogueDecoratorPlayMusicData : DialogueDecoratorData
{
    public AudioClip clip;
}

[System.Serializable]
public struct DialogueDecoratorCameraShakeData : DialogueDecoratorData
{
    
}

