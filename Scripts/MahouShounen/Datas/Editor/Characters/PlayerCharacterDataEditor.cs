using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(PlayerCharacterData))]
public class PlayerDataEditor : Editor
{
    PlayerCharacterData playerCharacterData;

    float lineHeight;
    float lineHeightSpace;

    // Portrait
    bool showPortrait = true;

    EmotionType emotionType;
    Sprite emotionSprite;

    PlayerClothingType clothingType;
    int clothingIndex;
    Sprite clothingSprite;

    Texture2D portraitTexture;
    int portraitTextureWidth = 1;

    // Weekend Event List
    bool showWeekendEventList = true;

    ReorderableList weekendEventList;
    SerializedProperty weekendEventsProperty;


    void OnEnable()
    {
        if (target != null)
            playerCharacterData = (PlayerCharacterData)target;

        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 10;

        // Weekend Event List
        weekendEventsProperty = serializedObject.FindProperty("_weekendEvents");
        weekendEventList = new ReorderableList(serializedObject, weekendEventsProperty, true, true, true, true);
        weekendEventList.elementHeight = 22;
        weekendEventList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Weekend Events");
        weekendEventList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            // Event Data Property
            EditorGUI.PropertyField(new Rect(rect.x + 2, rect.y + 2, rect.width - 2, 18), weekendEventList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        serializedObject.Update();

        // Icon Sprite
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Icon Sprite", GUILayout.Width(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_iconSprite"), GUIContent.none, true, GUILayout.MinWidth(96));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // Portrait
        showPortrait = EditorGUILayout.Foldout(showPortrait, "Portrait");
        if (showPortrait)
        {
            bool bIsNeedToRebuildPortraitTexture = false;

            // Portrait - Emotion
            EditorGUILayout.BeginHorizontal();
            {
                EmotionType prevEmotionType = emotionType;
                Sprite prevEmotionSprite = emotionSprite;

                emotionType = (EmotionType)EditorGUILayout.EnumPopup(emotionType, GUILayout.Width(160));
                if (playerCharacterData.emotionSprites.Count > (int)emotionType)
                    emotionSprite = playerCharacterData.emotionSprites[(int)emotionType];

                emotionSprite = (Sprite)EditorGUILayout.ObjectField(emotionSprite, typeof(Sprite), false);
                if (playerCharacterData.emotionSprites.Count > (int)emotionType)
                    playerCharacterData.emotionSprites[(int)emotionType] = emotionSprite;

                if (prevEmotionType   != emotionType ||
                    prevEmotionSprite != emotionSprite)
                {
                    bIsNeedToRebuildPortraitTexture = true;
                    EditorUtility.SetDirty(playerCharacterData);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Portrait - Costume
            string[] costumeIndexString = { "1번 의상", "2번 의상" , "3번 의상" };
            EditorGUILayout.BeginHorizontal();
            {
                PlayerClothingType prevClothingType = clothingType;

                int    prevClothingIndex  = clothingIndex;
                Sprite prevClothingSprite = clothingSprite;


                clothingType = (PlayerClothingType)EditorGUILayout.EnumPopup(clothingType, GUILayout.Width(92));
                if (prevClothingType != clothingType)
                {
                    clothingIndex = 0;

                    if (clothingType == PlayerClothingType.CasualWear)
                    {
                        if (playerCharacterData.casualWearSprites.Count > clothingIndex)
                            clothingSprite = playerCharacterData.casualWearSprites[clothingIndex];
                    }
                    else
                    {
                        if (playerCharacterData.costumeSprites.Count > clothingIndex)
                            clothingSprite = playerCharacterData.costumeSprites[clothingIndex];
                    }
                }

                clothingIndex = EditorGUILayout.Popup(clothingIndex, costumeIndexString, GUILayout.Width(65));
                if (playerCharacterData.costumeSprites.Count > clothingIndex)
                {
                    if (clothingType == PlayerClothingType.CasualWear)
                        clothingSprite = playerCharacterData.casualWearSprites[clothingIndex];
                    else
                        clothingSprite = playerCharacterData.costumeSprites[clothingIndex];
                }

                clothingSprite = (Sprite)EditorGUILayout.ObjectField(clothingSprite, typeof(Sprite), false);
                if (playerCharacterData.costumeSprites.Count > clothingIndex)
                {
                    if (clothingType == PlayerClothingType.CasualWear)
                        playerCharacterData.casualWearSprites[clothingIndex] = clothingSprite;
                    else
                        playerCharacterData.costumeSprites[clothingIndex] = clothingSprite;
                }


                if (prevClothingType   != clothingType ||
                    prevClothingIndex  != clothingIndex ||
                    prevClothingSprite != clothingSprite)
                {
                    bIsNeedToRebuildPortraitTexture = true;
                    EditorUtility.SetDirty(playerCharacterData);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Draw PortraitRect
            var portraitRectWidth  = (int)EditorGUILayout.GetControlRect().width;
            var portraitRectHeight = 270;

            if (portraitRectWidth != 1 &&
                portraitRectWidth != portraitTextureWidth)
            {
                portraitTextureWidth = portraitRectWidth;
                MakePortraitTexture(portraitTextureWidth, portraitRectHeight);
            }

            if (portraitTexture == null ||
                bIsNeedToRebuildPortraitTexture)
                MakePortraitTexture(portraitTextureWidth, portraitRectHeight);

            GUIStyle portraitStyle = new GUIStyle();
            portraitStyle.normal.background = portraitTexture;

            EditorGUILayout.LabelField(GUIContent.none, portraitStyle, GUILayout.Width(portraitTextureWidth), GUILayout.Height(portraitRectHeight));
            EditorGUILayout.LabelField(GUIContent.none);
        }

        // Weekend Event List
        showWeekendEventList = EditorGUILayout.Foldout(showWeekendEventList, "Weekend Event List");
        if (showWeekendEventList)
            weekendEventList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void MakePortraitTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        // Set Background White
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (((x / 16) % 2) - ((y / 16) % 2) == 0)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, new Color(0.75f, 0.75f, 0.75f, 1.0f));
            }
        }

        // Add Merging Textures
        List<Sprite> mergingSprites = new List<Sprite>();
        if (emotionSprite != null)
            mergingSprites.Add(emotionSprite);
        if (clothingSprite != null)
            mergingSprites.Add(clothingSprite);

        // Merging Texture
        foreach (var mergingSprite in mergingSprites)
        {
            if (mergingSprite == null)
                continue;

            int xStart = (width - mergingSprite.texture.width) / 2;
            int yStart = height - mergingSprite.texture.height;

            for (int x = 0; x < mergingSprite.texture.width; x++)
            {
                if (x + xStart < 0 ||
                    x + xStart >= width)
                    continue;

                for (int y = 0; y < mergingSprite.texture.height; y++)
                {
                    if (y + yStart < 0 ||
                        y + yStart >= height)
                        continue;

                    var color = Color.Lerp(texture.GetPixel(x + xStart, y + yStart),
                                           mergingSprite.texture.GetPixel(x, y),
                                           mergingSprite.texture.GetPixel(x, y).a);

                    texture.SetPixel(x + xStart, y + yStart, color);
                }
            }
        }

        texture.Apply();
        portraitTexture = texture;
    }
}
