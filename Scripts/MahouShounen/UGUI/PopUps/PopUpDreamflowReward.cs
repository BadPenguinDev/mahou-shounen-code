using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDreamflowReward : PopUpBase
{
    public GameObject prefabRewardElement;

    public RectTransform contentRewardSlots;
    public Button buttonGetReward;

    DreampostData dreampost;

    List<ElementDreamflowReward>   activeRewardElements;
    List<ElementDreamflowReward> inactiveRewardElements;


    public override void SetManager(UGUIManager uiManager)
    {
        base.SetManager(uiManager);

          activeRewardElements = new List<ElementDreamflowReward>();
        inactiveRewardElements = new List<ElementDreamflowReward>();

        buttonGetReward.onClick.AddListener(OnButtonGetRewardClicked);
    }
    public void SetDreampostData(DreampostData data)
    {
        dreampost = data;
        
        contentRewardSlots.sizeDelta = new Vector2(8f, 0);
        
        foreach (var element in activeRewardElements)
        {
            element.gameObject.SetActive(false);
            inactiveRewardElements.Add(element);
        }
        activeRewardElements.Clear();
        
        for (var i = 0; i < dreampost.rewards.Count; i++)
        {
            ElementDreamflowReward element;
            RectTransform elementTransform;

            if (inactiveRewardElements.Count > 0)
            {
                element = inactiveRewardElements[0];
                element.gameObject.SetActive(true);
                elementTransform = element.GetComponent<RectTransform>();

                inactiveRewardElements.RemoveAt(0);
            }
            else
            {
                var instance = Instantiate(prefabRewardElement, contentRewardSlots);

                element = instance.GetComponent<ElementDreamflowReward>();
                element.SetParentComponent(this);

                elementTransform = element.GetComponent<RectTransform>();
            }
            activeRewardElements.Add(element);

            var elementWidth = elementTransform.sizeDelta.x + 4f;

            element.SetRewardData(dreampost.rewards[i]);
            elementTransform.anchoredPosition = new Vector2(4f + (elementWidth * i), 0f);

            contentRewardSlots.sizeDelta += new Vector2(elementWidth, 0f);
        }
        contentRewardSlots.anchoredPosition = Vector2.zero;
    }

    void OnButtonGetRewardClicked()
    {
        foreach (var reward in dreampost.rewards)
        {
            MSGameInstance.Get().ChangeItemCount(new Item(reward.item, reward.amount));
        }
        MSGameInstance.Get().AddDreampostCount(dreampost);

        ClosePopUp();
        manager.CloseLayout(LayoutType.Dreamflow);

        manager.OpenLayout(LayoutType.Main);
        manager.SetLayoutMainMode(LayoutMainMode.Schedule);
    }
}
