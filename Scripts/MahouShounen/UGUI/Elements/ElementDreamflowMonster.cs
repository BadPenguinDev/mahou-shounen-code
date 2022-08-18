using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementDreamflowMonster : ElementBase
{
    [SerializeField] Image imageSprite;

    BattleFigureData figureData;
    IEnumerator animCoroutine;


    public void SetFigureData(BattleFigureData battlefigureData) 
    {
        figureData = battlefigureData;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        if (battlefigureData != null)
        {
            imageSprite.gameObject.SetActive(true);

            animCoroutine = Anim();
            StartCoroutine(animCoroutine);
        }
        else
        {
            imageSprite.gameObject.SetActive(false);
        }
    }

    IEnumerator Anim()
    {
        int spriteIndex = 0; 
        int spriteCount = figureData.idleSprite.Count; 

        imageSprite.GetComponent<RectTransform>().sizeDelta = figureData.spriteSize; 

        // Firing Sequence
        while (true)
        { 
            spriteIndex = 0; 
            imageSprite.sprite = figureData.idleSprite[0]; 

            for (float i = 0; i < 1.0f; i += Time.deltaTime / figureData.idleTimer) 
            { 
                int index = (int)(i * spriteCount); 
                if (spriteIndex < index) 
                { 
                    spriteIndex = index; 
                    imageSprite.sprite = figureData.idleSprite[index]; 
                } 
                yield return null; 
            } 
        } 
    }
}
