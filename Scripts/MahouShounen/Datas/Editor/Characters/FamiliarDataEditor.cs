using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(FamiliarData))]
public class FamiliarDataEditor : Editor
{
    FamiliarData familiarData;

    float lineHeight;
    float lineHeightSpace;

    // Portrait
    bool showPortrait = true;

    EmotionType emotionType;
    Sprite emotionSprite;

    FamiliarFormType formType;
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
            familiarData = (FamiliarData)target;

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
                FamiliarFormType prevFormType = formType;
                EmotionType prevEmotionType   = emotionType;
                Sprite      prevEmotionSprite = emotionSprite;

                formType = (FamiliarFormType)EditorGUILayout.EnumPopup(formType, GUILayout.Width(65));

                emotionType = (EmotionType)EditorGUILayout.EnumPopup(emotionType, GUILayout.Width(92));
                if (formType == FamiliarFormType.Familiar)
                {
                    if (familiarData.familiarEmotionSprites.Count > (int)emotionType)
                        emotionSprite = familiarData.familiarEmotionSprites[(int)emotionType];
                }
                else
                {
                    if (familiarData.humanEmotionSprites.Count > (int)emotionType)
                        emotionSprite = familiarData.humanEmotionSprites[(int)emotionType];
                }

                emotionSprite = (Sprite)EditorGUILayout.ObjectField(emotionSprite, typeof(Sprite), false);
                if (formType == FamiliarFormType.Familiar)
                {
                    if (familiarData.familiarEmotionSprites.Count > (int)emotionType)
                        familiarData.familiarEmotionSprites[(int)emotionType] = emotionSprite;
                }
                else
                {
                    if (familiarData.humanEmotionSprites.Count > (int)emotionType)
                        familiarData.humanEmotionSprites[(int)emotionType] = emotionSprite;
                }

                if (prevEmotionType   != emotionType ||
                    prevEmotionSprite != emotionSprite ||
                    prevFormType      != formType)
                {
                    bIsNeedToRebuildPortraitTexture = true;
                    EditorUtility.SetDirty(familiarData);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Portrait - Costume
            if (formType == FamiliarFormType.Human)
            {
                string[] costumeIndexString = { "1번 의상", "2번 의상" };
                EditorGUILayout.BeginHorizontal();
                {
                    int prevClothingIndex = clothingIndex;
                    Sprite prevClothingSprite = clothingSprite;

                    clothingIndex = EditorGUILayout.Popup(clothingIndex, costumeIndexString, GUILayout.Width(160));
                    if (familiarData.costumeSprites.Count > clothingIndex)
                        clothingSprite = familiarData.costumeSprites[clothingIndex];

                    clothingSprite = (Sprite)EditorGUILayout.ObjectField(clothingSprite, typeof(Sprite), false);
                    if (familiarData.costumeSprites.Count > clothingIndex)
                        familiarData.costumeSprites[clothingIndex] = clothingSprite;

                    if (prevClothingIndex != clothingIndex ||
                        prevClothingSprite != clothingSprite)
                    {
                        bIsNeedToRebuildPortraitTexture = true;
                        EditorUtility.SetDirty(familiarData);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // Draw PortraitRect
            int portraitRectWidth = (int)EditorGUILayout.GetControlRect().width;
            int portraitRectHeight = 270;

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

        if (formType == FamiliarFormType.Human &&
            clothingSprite != null)
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
