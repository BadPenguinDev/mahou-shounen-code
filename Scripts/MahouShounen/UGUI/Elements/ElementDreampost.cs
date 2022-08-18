using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ElementDreampost : ElementBase
{
    public List<Sprite> spritePost;
    public Image imagePortraitMini;

    DreampostData dreampost;

    Button buttonDreampost;
    Image   imageDreampost;

    public override void SetParentComponent(MSUIComponentBase component)
    {
        base.SetParentComponent(component);

        buttonDreampost = GetComponent<Button>();
         imageDreampost = GetComponent<Image>();

        buttonDreampost.onClick.AddListener(OnButtonDreampostClicked);
    }

    public DreampostData GetDreampostData() { return dreampost; }
    public void SetDreampostData(DreampostData dreampostData)
    {
        dreampost = dreampostData;

        if (dreampostData == null)
        {
            imageDreampost.sprite = spritePost[1];
            imagePortraitMini.gameObject.SetActive(false);

            buttonDreampost.interactable = false;
        }
        else
        {
            imageDreampost.sprite = spritePost[0];
            imagePortraitMini.sprite = dreampostData.GetPortraitMini();
            imagePortraitMini.gameObject.SetActive(true);

            buttonDreampost.interactable = true;
        }
    }

    public void OnButtonDreampostClicked()
    {
        LayoutDreampost layout = parentComponent as LayoutDreampost;
        layout.SelectElementDreampost(this);
    }
    
}
