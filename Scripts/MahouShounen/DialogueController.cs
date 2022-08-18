using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;


public class DialogueController : MonoBehaviour
{
    #region UI BInding
    [Header("UI Binding")]
    [SerializeField] private Image _portraitLeftImage;
    [SerializeField] private Image _clothingLeftImage;
    [SerializeField] private Image _portraitRightImage;
    [SerializeField] private Image _clothingRightImage;

    [Space(10f)]
    [SerializeField] private Image _frameCGImage;
    [SerializeField] private Image _fullCGImage;

    [Space(10f)]
    [SerializeField] private Text _textName;
    [SerializeField] private Text _textDialogue;

    [Space(10f)]
    [SerializeField] private RectTransform _layoutDialogue;
    [SerializeField] private RectTransform _dialogueBox;
    [SerializeField] private RectTransform _textNameArea;
    [SerializeField] private RectTransform _branchArea;
    [SerializeField] private RectTransform _animationCGArea;
    [SerializeField] private RectTransform _transitionArea;
    
    [Space(10f)]
    [SerializeField] private List<Text> _branchTexts;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   dialogueAudioClip;

    bool isDialogueSoundPlaying = false;
    IEnumerator playDialogueSoundCoroutine;

    public Image portraitLeftImage
    {
        get { return  _portraitLeftImage; }
        private set { _portraitLeftImage = value; }
    }
    public Image clothingLeftImage
    {
        get { return  _clothingLeftImage; }
        private set { _clothingLeftImage = value; }
    }
    public Image portraitRightImage
    {
        get { return  _portraitRightImage; }
        private set { _portraitRightImage = value; }
    }
    public Image clothingRightImage
    {
        get { return  _clothingRightImage; }
        private set { _clothingRightImage = value; }
    }

    public Image frameCGImage
    {
        get { return _frameCGImage; }
        private set { _frameCGImage = value; }
    }
    public Image fullCGImage
    {
        get { return _fullCGImage; }
        private set { _fullCGImage = value; }
    }

    public Text textName
    {
        get { return _textName; }
        private set { _textName = value; }
    }
    public Text textDialogue
    {
        get { return _textDialogue; }
        private set { _textDialogue = value; }
    }

    public RectTransform layoutDialogue
    {
        get { return  _layoutDialogue; }
        private set { _layoutDialogue = value; }
    }
    public RectTransform dialogueBox
    {
        get { return _dialogueBox; }
        private set { _dialogueBox = value; }
    }
    public RectTransform textNameArea
    {
        get { return _textNameArea; }
        private set { _textNameArea = value; }
    }
    public RectTransform branchArea
    {
        get { return _branchArea; }
        private set { _branchArea = value; }
    }
    public RectTransform animationCGArea
    {
        get { return _animationCGArea; }
        private set { _animationCGArea = value; }
    }
    public RectTransform transitionArea
    {
        get { return _transitionArea; }
        private set { _transitionArea = value; }
    }

    public List<Text> branchTexts
    {
        get { return _branchTexts; }
        private set { _branchTexts = value; }
    }
    #endregion

    #region Dialogue Setting
    [Header("Dialogue Setting")]
    [SerializeField] private DialogueContainer _targetDialogueContainer;
    [SerializeField] private float  _dialogueSpeed = 0.0f;
    [SerializeField] private Sprite _spriteTransparent;
    [SerializeField] private float  _fullCGTransitionTimer = 0.25f;

    
    public DialogueContainer targetDialogueContainer
    {
        get { return _targetDialogueContainer; }
        private set { _targetDialogueContainer = value; }
    }
    public float dialogueSpeed
    {
        get { return _dialogueSpeed; }
        private set { _dialogueSpeed = value; }
    }
    public Sprite spriteTransparent
    {
        get { return _spriteTransparent; }
        private set { _spriteTransparent = value; }
    }
    public float fullCGTransitionTimer
    {
        get { return _fullCGTransitionTimer; }
        private set { _fullCGTransitionTimer = value; }
    }

    IEnumerator fullCGTransitionCoroutine;

    // AnimPos
    public List<RectTransform> animRectTransforms;
    private List<Vector2> animOriginalPositions = new List<Vector2>();
    
    IEnumerator playCameraShakeEffectCoroutine;
    #endregion

    DialogueNodeDataBase currentNodeData;
    float dialogueIndex = 0;
    bool  fullCGSkipped = true;
    bool  animationCompleted = true;
    bool   waitingTransition = false;
    bool    dialogueFinished = false;

    string targetString = "";
    GameObject targetAnimationCG;

    
    private UGUIManager uiManager;
    private MSGameController gameController;

    
    private void Awake()
    {
        foreach (var rectTransform in animRectTransforms)
        {
            animOriginalPositions.Add(rectTransform.anchoredPosition);
        }

        uiManager = GetComponent<UGUIManager>();
        gameController = GetComponent<MSGameController>();
    }

    void Update()
    {
        if (waitingTransition)
            return;

        if (!fullCGSkipped) 
            return;

        if (!(dialogueIndex < targetString.Length)) 
            return;

        var prevIndex = (int)dialogueIndex;
        dialogueIndex += dialogueSpeed * Time.deltaTime;

        if ((int)dialogueIndex == prevIndex) 
            return;
        
        textDialogue.text = targetString.Substring(0, (int)dialogueIndex);
        
        var c = textDialogue.text.ToCharArray()[(int)dialogueIndex - 1];
        if (CheckCharacterPlayingSound(c))
            StartPlayDialogueSound();
    }

    public void SetTargetDialogueContainer(DialogueContainer container)
    {
        portraitLeftImage.sprite = spriteTransparent;
        clothingLeftImage.sprite = spriteTransparent;
        portraitRightImage.sprite = spriteTransparent;
        clothingRightImage.sprite = spriteTransparent;
        frameCGImage.sprite = spriteTransparent;
        fullCGImage.sprite = spriteTransparent;
        textName.text = "";
        textDialogue.text = "";

        dialogueIndex = 0;
        fullCGSkipped = true;
        animationCompleted = true;
        dialogueFinished = false;
        targetString = "";

        targetDialogueContainer = container;
        currentNodeData = targetDialogueContainer.GetEntryPointNode();
        
        dialogueBox.gameObject.SetActive(true);

        ProgressDialogue();
    }
    public void ProgressDialogue()
    {
        if (waitingTransition)
            return;

        if (dialogueFinished)
            return;

        if (fullCGSkipped)
        {
            if (dialogueIndex < targetString.Length)
            {
                // Animation CG Play
                if (targetAnimationCG != null)
                {
                    if (targetAnimationCG.GetComponent<Animator>().speed == 1.0f)
                    {
                        targetAnimationCG.GetComponent<Animator>().speed = 8.0f;
                        return;
                    }
                }

                dialogueIndex = targetString.Length;
                textDialogue.text = targetString.Substring(0, (int)dialogueIndex);
            }
            else
            {
                if (animationCompleted != true) 
                    return;
                
                if (targetAnimationCG != null)
                {
                    Destroy(targetAnimationCG);
                    targetAnimationCG = null;
                }

                if (targetDialogueContainer.GetNextNode(currentNodeData).GetType() == typeof(ExitPointNodeData))
                {
                    if (targetDialogueContainer.playTransition)
                        gameController.PlayTransition();
                    else
                    {
                        dialogueFinished = true;
                        uiManager.OnDialogueFinished();
                    }
                }
                else
                    SetCurrentNode(targetDialogueContainer.GetNextNode(currentNodeData));
            }
        }
        else
        {
            fullCGSkipped = true;
            dialogueBox.gameObject.SetActive(true);
        }
    }
    public void ProgressBranch(int index)
    {
        var nodeData = (BranchNodeData)currentNodeData;
        var links = targetDialogueContainer.GetBranches(nodeData);

        SetCurrentNode(targetDialogueContainer.GetNextNode(links[index]));

        branchArea.gameObject.SetActive(false);
    }
    public void SkipDialogue()
    {
        if (waitingTransition)
            return;

        if (dialogueFinished)
            return;
        
        var nodeData = targetDialogueContainer.GetNextNode(currentNodeData);
        while (true)
        {
            if (nodeData.GetType() == typeof(ExitPointNodeData))
            {
                if (targetDialogueContainer.playTransition)
                    gameController.PlayTransition();
                else
                {
                    dialogueFinished = true;
                    uiManager.OnDialogueFinished();
                }
                return;
            }

            nodeData = targetDialogueContainer.GetNextNode(nodeData);
        }
    }

    private void SetCurrentNode(DialogueNodeDataBase data)
    {
        currentNodeData = targetDialogueContainer.GetNextNode(currentNodeData);

        if (currentNodeData == null)
            return;

        if (currentNodeData.GetType() == typeof(ExitPointNodeData))
        {
            if (targetDialogueContainer.playTransition)
                gameController.PlayTransition();

            return;
        }
        else if (currentNodeData.GetType() == typeof(DialogueNodeData))
        {
            DialogueNodeData nodeData = (DialogueNodeData)currentNodeData;

            DialogueDecoratorData transitionDecoratorBase = nodeData.DecoratorList.Find(x => x.GetType() == typeof(DialogueDecoratorTransitionData));
            if (transitionDecoratorBase != null)
            {
                DialogueDecoratorTransitionData transitionDecorator = (DialogueDecoratorTransitionData)transitionDecoratorBase;
                targetAnimationCG = Instantiate(transitionDecorator.Transition, transitionArea);
                targetAnimationCG.GetComponent<TransitionComponent>().DialogueController = this;

                waitingTransition = true;
                return;
            }

            textDialogue.text = "";
            PlayDialogueNode();
        }
        else if (currentNodeData.GetType() == typeof(BranchNodeData))
        {
            var nodeData = (BranchNodeData)currentNodeData;
            var links = targetDialogueContainer.GetBranches(nodeData);
            for (var i = 0; i < branchTexts.Count; i++)
            {
                // string branchString = LocalizationSystem.Get().GetTranslatedString("Dialogues", links[i].PortName);
                var branchString = LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", links[i].PortName);
                branchTexts[i].text = branchString;
            }
            branchArea.gameObject.SetActive(true);
        }
    }
    private void SetPortrait(PortraitType portraitType, EmotionType emotionType, PortraitOrientation orientation)
    {
        Sprite clothingSprite = null;
        Sprite  emotionSprite = null;

        // Get Portrait
        GetPortraitSpriteByPortraitType(portraitType, emotionType, ref clothingSprite, ref emotionSprite);

        if (clothingSprite == null)
            clothingSprite = spriteTransparent;

        if (emotionSprite == null)
            emotionSprite = spriteTransparent;

        // Set Portrait
        if (orientation == PortraitOrientation.Left)
        {
            portraitLeftImage.sprite =  emotionSprite;
            clothingLeftImage.sprite = clothingSprite;
        }
        else if (orientation == PortraitOrientation.Right)
        {
            portraitRightImage.sprite =  emotionSprite;
            clothingRightImage.sprite = clothingSprite;
        }

        switch (portraitType)
        {
            case PortraitType.None:
                break;
            case PortraitType.Shounen:
            case PortraitType.ShounenMagical:
                textName.GetComponent<LocalizeStringEvent>().enabled = false;
                textName.text = MSGameInstance.Get().playerName;
                break;
            default:
                textName.GetComponent<LocalizeStringEvent>().enabled = true;
                textName.GetComponent<LocalizeStringEvent>().SetTable("Characters");
                textName.GetComponent<LocalizeStringEvent>().SetEntry(portraitType.ToString());
                break;
        }

        textNameArea.gameObject.SetActive(true);
    }
    private void PlayDialogueNode()
    {
        var nodeData = (DialogueNodeData)currentNodeData;

        targetString = LocalizationSettings.StringDatabase.GetLocalizedString("Dialogues", nodeData.DialogueKey);
        // targetString = LocalizationSystem.Get().GetTranslatedString("Dialogues", nodeData.DialogueKey);
        // targetString = targetString.Replace("@p", MSGameInstance.Get().playerName);

        dialogueIndex = 0;
        textNameArea.gameObject.SetActive(false);

        // Get Target Portrait
        var portraitTargeted = new Dictionary<PortraitOrientation, bool>
        {
            { PortraitOrientation.Left,  false },
            { PortraitOrientation.Right, false }
        };

        foreach (var decorator in nodeData.DecoratorList)
        {
            switch (decorator)
            {
                case DialogueDecoratorSpeechData speechDecorator:
                    SetPortrait(speechDecorator.PortraitType, speechDecorator.EmotionType, speechDecorator.PortraitOrientation);
                    portraitTargeted[speechDecorator.PortraitOrientation] = true;
                    break;
                
                case DialogueDecoratorFrameCGData frameCGDecorator:
                    frameCGImage.sprite = frameCGDecorator.SpriteFrameCG;
                    break;
                
                case DialogueDecoratorFullCGData fullCgDecorator:
                {
                    fullCGImage.sprite = fullCgDecorator.SpriteFullCG;

                    if (fullCgDecorator.isIgnoreDialogue) 
                        continue;

                    StartFullCGTransition();
                    fullCGSkipped = false;
                    dialogueBox.gameObject.SetActive(false);
                    break;
                }
                case DialogueDecoratorAnimationCGData animationCGDecorator:
                {
                    targetAnimationCG = Instantiate(animationCGDecorator.AnimationCG, animationCGArea);
                    targetAnimationCG.GetComponent<AnimationCGComponent>().DialogueController = this;

                    if (animationCGDecorator.isIgnoreDialogue) 
                        continue;
                
                    animationCompleted = false;
                    fullCGSkipped = false;
                    dialogueBox.gameObject.SetActive(false);
                    break;
                }
                
                case DialogueDecoratorPlaySFXData playSFXDecorator:
                    uiManager.PlayAudioClip(playSFXDecorator.clip);
                    break;

                case DialogueDecoratorPlayMusicData playMusicDecorator:
                {
                    var clip = playMusicDecorator.clip;
                    if (clip == null)
                        gameController.StopEventMusic();
                    else
                        gameController.PlayEventMusic(clip);
                    break;
                }
                case DialogueDecoratorCameraShakeData cameraShakeDecorator:
                    StartPlayCameraShakeEffect();
                    break;
            }
        }

        // Set Target Portrait
        if (portraitTargeted[PortraitOrientation.Left] == false)
        {
            portraitLeftImage.color = Color.grey;
            clothingLeftImage.color = Color.grey;
        }
        else
        {
            portraitLeftImage.color = Color.white;
            clothingLeftImage.color = Color.white;
        }

        if (portraitTargeted[PortraitOrientation.Right] == false)
        {
            portraitRightImage.color = Color.grey;
            clothingRightImage.color = Color.grey;
        }
        else
        {
            portraitRightImage.color = Color.white;
            clothingRightImage.color = Color.white;
        }
    }

    // External
    public void FinishAnimation()
    {
        fullCGSkipped = true;
        animationCompleted = true;
        dialogueBox.gameObject.SetActive(true);
        targetAnimationCG.GetComponent<Animator>().speed = 0.0f;
    }
    public void FinishDialogue()
    {
        layoutDialogue.GetComponent<Animator>().enabled = true;
        layoutDialogue.GetComponent<Animator>().Play("Off");
    }

    // Delegate
    public void OnTransitionTriggered()
    {
        waitingTransition = false;
        PlayDialogueNode();
    }
    
    // Coroutine
    private void StartFullCGTransition()
    {
        if (fullCGTransitionCoroutine != null)
            StopCoroutine(fullCGTransitionCoroutine);

        fullCGTransitionCoroutine = FullCGTransition(fullCGImage.sprite == spriteTransparent);

        StartCoroutine(fullCGTransitionCoroutine);
    }
    private IEnumerator FullCGTransition(bool isOut)
    {
        fullCGImage.color = new Color(1f, 1f, 1f, 0f);
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / fullCGTransitionTimer)
        {
            fullCGImage.color = isOut ? new Color(1f, 1f, 1f, 1f - t) : 
                                        new Color(1f, 1f, 1f, t);
            yield return null;
        }
        fullCGImage.color = isOut ? new Color(1f, 1f, 1f, 0f) : 
                                    new Color(1f, 1f, 1f, 1f);
    }

    private bool CheckCharacterPlayingSound(char c)
    {
        var excludeCharLists = " ,.!?-~()'\"";
        return !excludeCharLists.ToCharArray().Contains(c);
    }
    private void StartPlayDialogueSound()
    {
        if (isDialogueSoundPlaying == true)
            return;
        
        if (playDialogueSoundCoroutine != null)
            StopCoroutine(playDialogueSoundCoroutine);

        playDialogueSoundCoroutine = PlayDialogueSound();

        StartCoroutine(playDialogueSoundCoroutine);
    }
    private IEnumerator PlayDialogueSound()
    {
        isDialogueSoundPlaying = true;
        audioSource.PlayOneShot(dialogueAudioClip, 0.2f * MSGameInstance.Get().preferences.volumeSound);
        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.01f)
        {
            yield return null;
        }
        isDialogueSoundPlaying = false;
    }
    

    private void StartPlayCameraShakeEffect()
    {
        if (playCameraShakeEffectCoroutine != null)
            StopCoroutine(playCameraShakeEffectCoroutine);

        playCameraShakeEffectCoroutine = PlayCameraShakeEffect();
        StartCoroutine(playCameraShakeEffectCoroutine);
    }
    private IEnumerator PlayCameraShakeEffect()
    {
        var randomModifier = new Vector2();

        // Shattering Sequence
        const int shatterCount = 64;
        var currentIndex = -1;
        for (float t = 0; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            var index = (int)(t * shatterCount);
            if (currentIndex < index)
            {
                currentIndex++;
                
                randomModifier.x = Random.Range(-2, 2);
                randomModifier.y = Random.Range(-4, 0);
                
                for (var i = 0; i < animRectTransforms.Count; i++)
                {
                    animRectTransforms[i].anchoredPosition = animOriginalPositions[i] + randomModifier;
                }
            }
            yield return null;
        }
        
        // End Fire
        for (var i = 0; i < animRectTransforms.Count; i++)
        {
            animRectTransforms[i].anchoredPosition = animOriginalPositions[i];
        }
    }
    
    public void GetPortraitSpriteByPortraitType(PortraitType portraitType, EmotionType emotionType, ref Sprite clothingSprite, ref Sprite emotionSprite)
    {
        switch (portraitType)
        {
            case PortraitType.None:
                clothingSprite = spriteTransparent;
                 emotionSprite = spriteTransparent;
                break;
            case PortraitType.Shounen:
            {
                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                var  emotionIndex = (int)emotionType;

                clothingSprite = MSDataManager.Get().playerCharacterData.casualWearSprites[clothingIndex];
                 emotionSprite = MSDataManager.Get().playerCharacterData.emotionSprites[emotionIndex];
                break;
            }
            case PortraitType.ShounenMagical:
            {
                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                var  emotionIndex = (int)emotionType;

                clothingSprite = MSDataManager.Get().playerCharacterData.costumeSprites[clothingIndex];
                 emotionSprite = MSDataManager.Get().playerCharacterData.emotionSprites[emotionIndex];
                break;
            }
            case PortraitType.Familiar:
            {
                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                var  emotionIndex = (int)emotionType;
            
                clothingSprite = null;
                 emotionSprite = MSDataManager.Get().familiarData.familiarEmotionSprites[emotionIndex];
                break;
            }
            case PortraitType.FamiliarHuman:
            {
                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                var  emotionIndex = (int)emotionType;
            
                clothingSprite = MSDataManager.Get().familiarData.costumeSprites[0];
                 emotionSprite = MSDataManager.Get().familiarData.humanEmotionSprites[emotionIndex];
                break;
            }
            default:
            {
                var type = GetFriendshipTypeByPortraitType(portraitType);
                var character = MSDataManager.Get().characterDatas.Find(x => x.friendshipType == type);

                if (character == null)
                {
                    clothingSprite = null;
                    emotionSprite  = null;

                    return;
                }

                var clothingIndex = (int)MSGameInstance.Get().casualwear;
                var emotionIndex  = (int)emotionType;

                clothingSprite = character.costumeSprites[0];
                emotionSprite  = character.emotionSprites[emotionIndex];
                break;
            }
        }
    }
    public FriendshipType GetFriendshipTypeByPortraitType(PortraitType portraitType)
    {
        switch (portraitType)
        {
            case PortraitType.Scientist:        return FriendshipType.Scientist;
            case PortraitType.Enterprise:       return FriendshipType.Enterprise;
            case PortraitType.ArcheryPlayer:    return FriendshipType.ArcheryPlayer;
            case PortraitType.Violinist:        return FriendshipType.Violinist;
            case PortraitType.Chef:             return FriendshipType.Chef;
            case PortraitType.Cartoonist:       return FriendshipType.Cartoonist;
            case PortraitType.Programmer:       return FriendshipType.Programmer;
            case PortraitType.Idol:             return FriendshipType.Idol;
            case PortraitType.Prince:           return FriendshipType.Prince;
            case PortraitType.Ballerino:        return FriendshipType.Ballerino;
            default:                            return FriendshipType.None;
        }
    }

    
}
