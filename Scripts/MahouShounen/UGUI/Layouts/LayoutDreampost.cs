using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class LayoutDreampost : LayoutBase
{
    public List<ElementDreampost> elementDreamposts;

    public Image imagePortraitIcon;
    public Text textName;
    public Text textDescription;

    public GameObject prefabRewardElement;
    public RectTransform contentRewardSlots;

    public Button buttonReturn;
    public Button buttonEnterDreamflow;

    ElementDreampost selectedElement;

    List<ElementDreamflowReward>   activeRewardElements;
    List<ElementDreamflowReward> inactiveRewardElements;
    
    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

        for (int i = 0; i < elementDreamposts.Count; i++)
        {
            elementDreamposts[i].SetParentComponent(this);
        }

          activeRewardElements = new List<ElementDreamflowReward>();
        inactiveRewardElements = new List<ElementDreamflowReward>();

        buttonReturn.onClick.AddListener(OnButtonReturnClicked);
        buttonEnterDreamflow.onClick.AddListener(OnButtonEnterDreamflowClicked);
    }

    public void LoadDreamposts()
    {
        var availableDreamposts = new List<DreampostData>();
        availableDreamposts = MSGameInstance.Get().GetAvailableDreampostDatas(elementDreamposts.Count);

        bool isSelected = false;
        for (var i = 0; i < availableDreamposts.Count; i++)
        {
            elementDreamposts[i].SetDreampostData(availableDreamposts[i]);

            if (isSelected || availableDreamposts[i] == null)
                continue;
            
            isSelected = true;
            SelectElementDreampost(elementDreamposts[i]);
        }
    }

    public void SelectElementDreampost(ElementDreampost element)
    {
        selectedElement = element;
        var data = selectedElement.GetDreampostData();

        // Default
        imagePortraitIcon.sprite = data.GetPortraitMini();

        textName.text        = data.postName.GetLocalizedString();
        textDescription.text = data.postDescription.GetLocalizedString();

        // Rewards
        contentRewardSlots.sizeDelta = new Vector2(8f, 0);

        foreach (var elementReward in activeRewardElements)
        {
            elementReward.gameObject.SetActive(false);
            inactiveRewardElements.Add(elementReward);
        }
        activeRewardElements.Clear();

        for (var i = 0; i < data.rewards.Count; i++)
        {
            ElementDreamflowReward elementReward;
            RectTransform elementTransform;

            if (inactiveRewardElements.Count > 0)
            {
                elementReward = inactiveRewardElements[0];
                elementReward.gameObject.SetActive(true);
                elementTransform = elementReward.GetComponent<RectTransform>();

                inactiveRewardElements.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabRewardElement, contentRewardSlots);

                elementReward = instance.GetComponent<ElementDreamflowReward>();
                elementReward.SetParentComponent(this);

                elementTransform = elementReward.GetComponent<RectTransform>();
            }
            activeRewardElements.Add(elementReward);

            float elementWidth = elementTransform.sizeDelta.x + 4f;

            elementReward.SetRewardData(data.rewards[i]);
            elementTransform.anchoredPosition = new Vector2(4f + (elementWidth * i), 0f);

            contentRewardSlots.sizeDelta += new Vector2(elementWidth, 0f);
        }
        contentRewardSlots.anchoredPosition = Vector2.zero;
    }


    public void OnButtonReturnClicked()
    {
        manager.OpenLayout(LayoutType.Main);
        CloseLayout();
    }
    public void OnButtonEnterDreamflowClicked()
    {
        LayoutDreamflow layoutDreamflow = manager.OpenLayout(LayoutType.Dreamflow) as LayoutDreamflow;
        layoutDreamflow.SetDreampostData(selectedElement.GetDreampostData());

        CloseLayout();
    }
}
