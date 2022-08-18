using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

// EDITOR
[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : Editor
{
    CharacterData characterData;

    float lineHeight;
    float lineHeightSpace;

    // Portrait
    bool showPortrait = true;

    EmotionType emotionType;
    Sprite emotionSprite;

    int costumeIndex;
    Sprite costumeSprite;

    Texture2D portraitTexture;
    int portraitTextureWidth = 1;

    // Character Schedule List
    const int characterScheduleTextureWidth  = 64;
    const int characterScheduleTextureHeight = 88;

    bool showCharacterScheduleList = true;

    // Gift Taste List
    const int giftTasteTextureWidth  = 48;
    const int giftTasteTextureHeight = 48;

    bool showGiftTasteList = true;
    Dictionary<ItemData, Sprite>  giftTasteSprites;
    Dictionary<Sprite, Texture2D> giftTasteTextures;

    ReorderableList    giftTasteList;
    SerializedProperty giftTastesProperty;

    // Weekend Event List
    bool showWeekendEventList = true;

    ReorderableList    weekendEventList;
    SerializedProperty weekendEventsProperty;


    void OnEnable()
    {
        if (target != null)
            characterData = (CharacterData)target;
        
        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 10;

        // Gift Taste 
        if (giftTasteSprites == null)
            giftTasteSprites = new Dictionary<ItemData, Sprite>();

        if (giftTasteTextures == null)
            giftTasteTextures = new Dictionary<Sprite, Texture2D>();

        giftTastesProperty = serializedObject.FindProperty("_giftTastes");
        giftTasteList = new ReorderableList(serializedObject, giftTastesProperty, true, true, true, true);
        giftTasteList.elementHeight = 52;
        giftTasteList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Gift Tastes");
        giftTasteList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = giftTasteList.serializedProperty.GetArrayElementAtIndex(index);

            // Item Sprite
            var itemData = (ItemData)element.FindPropertyRelative("_item").objectReferenceValue;
            if (!giftTasteSprites.ContainsKey(itemData))
                 giftTasteSprites.Add(itemData, itemData.sprite);

            Texture2D texture;
            if (giftTasteTextures.ContainsKey(itemData.sprite))
            {
                texture = giftTasteTextures[itemData.sprite];
            }
            else
            {
                if (itemData != null)
                    texture = GetGiftTasteTexture(itemData.sprite);
                else
                    texture = GetGiftTasteTexture(null);

                giftTasteTextures.Add(itemData.sprite, texture);
            }

            EditorGUI.DrawPreviewTexture(new Rect(rect.x + 2, rect.y + 2, giftTasteTextureWidth, giftTasteTextureHeight), texture);

            // Item Data Property
            var itemDataProperty = element.FindPropertyRelative("_item");
            EditorGUI.LabelField(new Rect(rect.x + giftTasteTextureWidth + 10, rect.y + 6, 64, 18), "Item Data");
            EditorGUI.PropertyField(new Rect(rect.x     +  giftTasteTextureWidth + 10 + 64, rect.y + 6, 
                                             rect.width - (giftTasteTextureWidth + 10 + 64), 18), itemDataProperty, GUIContent.none);

            // Gift Taste Property
            var giftTasteTypeProperty = element.FindPropertyRelative("_taste");
            EditorGUI.LabelField(new Rect(rect.x + giftTasteTextureWidth + 10, rect.y + 26, 64, 23), "Gift Taste");
            EditorGUI.PropertyField(new Rect(rect.x     +  giftTasteTextureWidth + 10 + 64, rect.y + 28, 
                                             rect.width - (giftTasteTextureWidth + 10 + 64), 22), giftTasteTypeProperty, GUIContent.none);
        };

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

        // Character Type
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Friendship Type", GUILayout.Width(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_friendshipType"), GUIContent.none, true, GUILayout.MinWidth(96));
        }
        EditorGUILayout.EndHorizontal();

        // Icon Sprite
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Icon Sprite", GUILayout.Width(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_iconSprite"), GUIContent.none, true, GUILayout.MinWidth(96));
        }
        EditorGUILayout.EndHorizontal();

        // Birthday
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Birthday", GUILayout.Width(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_month"), GUIContent.none, true, GUILayout.MinWidth(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_week"),  GUIContent.none, true, GUILayout.MinWidth(96));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_day"),   GUIContent.none, true, GUILayout.MinWidth(96));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // Portrait
        showPortrait = EditorGUILayout.Foldout(showPortrait, "Portrait");
        if (showPortrait)
        {
            var bIsNeedToRebuildPortraitTexture = false;

            // Portrait - Emotion
            EditorGUILayout.BeginHorizontal();
            {
                var prevEmotionType = emotionType;
                var prevEmotionSprite = emotionSprite;

                emotionType = (EmotionType)EditorGUILayout.EnumPopup(emotionType, GUILayout.Width(128));
                if (characterData.emotionSprites.Count > (int)emotionType)
                    emotionSprite = characterData.emotionSprites[(int)emotionType];

                emotionSprite = (Sprite)EditorGUILayout.ObjectField(emotionSprite, typeof(Sprite), false);
                if (characterData.emotionSprites.Count > (int)emotionType)
                    characterData.emotionSprites[(int)emotionType] = emotionSprite;

                if (prevEmotionType != emotionType ||
                    prevEmotionSprite != emotionSprite)
                {
                    bIsNeedToRebuildPortraitTexture = true;
                    EditorUtility.SetDirty(characterData);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Portrait - Costume
            string[] costumeIndexString = { "1번 의상", "2번 의상" };
            EditorGUILayout.BeginHorizontal();
            {
                var prevCostumeIndex = costumeIndex;
                var prevCostumeSprite = costumeSprite;

                costumeIndex = EditorGUILayout.Popup(costumeIndex, costumeIndexString, GUILayout.Width(128));
                if (characterData.costumeSprites.Count > costumeIndex)
                    costumeSprite = characterData.costumeSprites[costumeIndex];

                costumeSprite = (Sprite)EditorGUILayout.ObjectField(costumeSprite, typeof(Sprite), false);
                if (characterData.costumeSprites.Count > costumeIndex)
                    characterData.costumeSprites[costumeIndex] = costumeSprite;

                if (prevCostumeIndex  != costumeIndex ||
                    prevCostumeSprite != costumeSprite)
                {
                    bIsNeedToRebuildPortraitTexture = true;
                    EditorUtility.SetDirty(characterData);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Draw PortraitRect
            var portraitRectWidth = (int)EditorGUILayout.GetControlRect().width;
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

            var portraitStyle = new GUIStyle();
            portraitStyle.normal.background = portraitTexture;

            EditorGUILayout.LabelField(GUIContent.none, portraitStyle, GUILayout.Width(portraitTextureWidth), GUILayout.Height(portraitRectHeight));
            EditorGUILayout.LabelField(GUIContent.none);
        }

        // Character Schedule
        showCharacterScheduleList = EditorGUILayout.Foldout(showCharacterScheduleList, "Character Schedule");
        if (showCharacterScheduleList)
        {
            // Header Style
            var headerStyle = GUI.skin.GetStyle("RL Header");
            headerStyle.fontSize = 12;
            headerStyle.alignment = TextAnchor.MiddleLeft;
            headerStyle.padding.top = 3;
            headerStyle.padding.left = 7;
            headerStyle.richText = true;
            EditorGUILayout.LabelField("Character Schedules", headerStyle);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var scheduleProperty = serializedObject.FindProperty("_characterSchedules");
                foreach (Day day in System.Enum.GetValues(typeof(Day)))
                {
                    if (day == Day.Sunday)
                        continue;

                    // Element Style
                    var elementStyle = new GUIStyle();
                    var elementTexture = new Texture2D(1, 1);
                    if ((int)day % 2 == 0)
                        elementTexture.SetPixel(1, 1, new Color(0.0f, 0.0f, 0.0f, 0.25f));
                    else                                                      
                        elementTexture.SetPixel(1, 1, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                    elementTexture.Apply();
                    elementStyle.normal.background = elementTexture;

                    // Item Sprite
                    var element = scheduleProperty.GetArrayElementAtIndex((int)day);
                    EditorGUILayout.BeginHorizontal(elementStyle);
                    {
                        var rowStyle = new GUIStyle();
                        rowStyle.padding.top = 3;
                        rowStyle.padding.bottom = 2;

                        EditorGUILayout.BeginHorizontal(rowStyle);
                        {
                            EditorGUILayout.LabelField(day.ToString(), GUILayout.Width(80));
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("_schedule"), GUIContent.none, GUILayout.ExpandWidth(true));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.LabelField(GUIContent.none, GUILayout.Height(1));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField(GUIContent.none);
        }

        // Gift Taste List
        showGiftTasteList = EditorGUILayout.Foldout(showGiftTasteList, "Gift Taste List");
        if (showGiftTasteList)
            giftTasteList.DoLayoutList();

        // Weekend Event List
        showWeekendEventList = EditorGUILayout.Foldout(showWeekendEventList, "Weekend Event List");
        if (showWeekendEventList)
            weekendEventList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void MakePortraitTexture(int width, int height)
    {
        var texture = new Texture2D(width, height);

        // Set Background White
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (((x / 16) % 2) - ((y / 16) % 2)  == 0)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, new Color(0.75f, 0.75f, 0.75f, 1.0f));
            }
        }

        // Add Merging Textures
        var mergingSprites = new List<Sprite>();
        if (emotionSprite != null)
            mergingSprites.Add(emotionSprite);
        if (costumeSprite != null)
            mergingSprites.Add(costumeSprite);

        // Merging Texture
        foreach (var mergingSprite in mergingSprites)
        {
            if (mergingSprite == null)
                continue;

            var xStart = (width - mergingSprite.texture.width) / 2;
            var yStart = height - mergingSprite.texture.height;

            for (var x = 0; x < mergingSprite.texture.width; x++)
            {
                if (x + xStart < 0 || 
                    x + xStart >= width)
                    continue;

                for (var y = 0; y < mergingSprite.texture.height; y++)
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
    Texture2D GetGiftTasteTexture(Sprite sprite)
    {
        var texture = new Texture2D(giftTasteTextureWidth, giftTasteTextureHeight);

        // Set Background White
        for (var x = 0; x < giftTasteTextureWidth; x++)
        {
            for (var y = 0; y < giftTasteTextureHeight; y++)
            {
                if (((x / 8) % 2) - ((y / 8) % 2) == 0)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, new Color(0.75f, 0.75f, 0.75f, 1.0f));
            }
        }

        if (sprite != null)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                var xStart = (giftTasteTextureWidth  - (int)sprite.rect.width)  / 2 + (int)sprite.textureRectOffset.x;
                var yStart = (giftTasteTextureHeight - (int)sprite.rect.height) / 2 + (int)sprite.textureRectOffset.y;

                for (var x = 0; x < (int)sprite.textureRect.width; x++)
                {
                    if (x + xStart <  0 ||
                        x + xStart >= giftTasteTextureWidth)
                        continue;

                    for (var y = 0; y < (int)sprite.textureRect.height; y++)
                    {
                        if (y + yStart <  0 ||
                            y + yStart >= giftTasteTextureHeight)
                            continue;

                        var color = Color.Lerp(texture.GetPixel(x + xStart, y + yStart), 
                                               sprite.texture.GetPixel((int)sprite.textureRect.x + x, (int)sprite.textureRect.y + y), 
                                               sprite.texture.GetPixel((int)sprite.textureRect.x + x, (int)sprite.textureRect.y + y).a);

                        texture.SetPixel(x + xStart, y + yStart, color);
                    }
                }
            }
            else
            {
                var xStart = (giftTasteTextureWidth  - sprite.texture.width)  / 2;
                var yStart = (giftTasteTextureHeight - sprite.texture.height) / 2;

                for (var x = 0; x < sprite.texture.width; x++)
                {
                    if (x + xStart <  0 || 
                        x + xStart >= giftTasteTextureWidth)
                        continue;

                    for (var y = 0; y < sprite.texture.height; y++)
                    {
                        if (y + yStart <  0 ||
                            y + yStart >= giftTasteTextureHeight)
                            continue;

                        var color = Color.Lerp(texture.GetPixel(x + xStart, y + yStart),
                                               sprite.texture.GetPixel(x, y), 
                                               sprite.texture.GetPixel(x, y).a);

                        texture.SetPixel(x + xStart, y + yStart, color);
                    }
                }
            }
        }

        texture.Apply();
        return texture;
    }
}
