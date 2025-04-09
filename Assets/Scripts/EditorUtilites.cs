using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class EditorUtils
{
    public static GUIStyle IconButton => GUI.skin.FindStyle("IconButton");

    // icons: https://github.com/halak/unity-editor-icons

    /// <summary>
    /// Draw custom label
    /// </summary>
    public static void DrawLabel(string title, GUIStyle style = null, float space = 0f)
    {
        EditorGUILayout.Space(space);
        Rect rect = GUILayoutUtility.GetRect(1f, 20f);
        rect.xMin += 20f;
        EditorGUI.LabelField(rect, title, style);
    }

    public static void DrawProperty(SerializedProperty property, string label = null, string toolTip = null, GUIStyle style = null, int space = 3)
    {
        string nameProperty = property.displayName;

        if (label != null)
        {
            if (style != null)
            {
                EditorGUILayout.Space(space);
                Rect newRect = EditorGUILayout.GetControlRect();
                newRect.xMin += 20f;
                EditorGUI.LabelField(newRect, new GUIContent(label), style);
            }
            else
            {
                nameProperty = label;
            }
        }

        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = GUILayoutUtility.GetRect(1f, height);
        rect.xMin += 20f;
        EditorGUI.PropertyField(rect, property, new GUIContent(nameProperty, toolTip));
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
    }

    /// <summary>
    /// Draw many propertys
    /// </summary>
    public static void DrawPropertys(params SerializedProperty[] propertys)
    {
        foreach (var property in propertys)
        {
            float height = EditorGUI.GetPropertyHeight(property, true);
            Rect rect = GUILayoutUtility.GetRect(1f, height);
            rect.xMin += 20f;
            EditorGUI.PropertyField(rect, property, new GUIContent(property.displayName));
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
    }

    /// <summary>
    /// Draw many properties with label
    /// </summary>
    public static void DrawPropertys(string label, GUIStyle labelStyle, params SerializedProperty[] propertys)
    {
        if (label != null)
        {
            DrawLabel(label, labelStyle);
        }

        foreach (var property in propertys)
        {
            DrawProperty(property);
        }
    }

    /// <summary>
    /// Show foldout for expanded property
    /// </summary>
    public static void DrawFoldoutExpandedProperty(string title, string icon, SerializedProperty property)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUIContent iconTitle =
            icon == "" ? EditorGUIUtility.TrTextContent(" " + title)
            : EditorGUIUtility.TrTextContentWithIcon(" " + title, icon);
        Rect rect = GUILayoutUtility.GetRect(1, 20f);

        property.isExpanded = DrawFoldoutHeader(rect, iconTitle, true, property.isExpanded);

        if (property.isExpanded)
        {
            EditorGUILayout.Space(3f);
            DrawRelativeProperties(property);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Show foldout for non expanded property
    /// </summary>
    public static void DrawFoldoutPropertys(string title, ref bool state, string icon = "", params SerializedProperty[] propertys)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            GUIContent iconTitle =
                icon == "" ? EditorGUIUtility.TrTextContent(" " + title)
                : EditorGUIUtility.TrTextContentWithIcon(" " + title, icon);
            Rect rect = GUILayoutUtility.GetRect(1, 20f);

            state = DrawFoldoutHeader(rect, iconTitle, true, state);

            if (state)
            {
                foreach (var property in propertys)
                {
                    if (property.isExpanded)
                    {
                        DrawRelativeProperties(property);
                    }
                    else
                    {
                        DrawProperty(property);
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draw many propertys from custom property and with tool tip
    /// </summary>
    public static void DrawCustomPropertys(params CustomProperty[] propertys)
    {
        foreach (var property in propertys)
        {
            DrawProperty(property.Property, property.Label, property.Tooltip, property.Style);
        }
    }

    /// <summary>
    /// Draw foldout with custom property
    /// </summary>
    public static void DrawFoldoutCustomPropertys(string titleFoldout, ref bool state, string icon = "", params CustomProperty[] propertys)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            GUIContent iconTitle = EditorGUIUtility.TrTextContent(" " + titleFoldout, icon);
            Rect rect = GUILayoutUtility.GetRect(1, 20f);

            state = DrawFoldoutHeader(rect, iconTitle, true, state);

            if (state)
            {
                foreach (var property in propertys)
                {
                    if (property.Property.isExpanded)
                    {
                        DrawRelativeProperties(property.Property, label: property.Label, style: property.Style);
                    }
                    else
                    {
                        DrawProperty(property.Property, property.Label, property.Tooltip, property.Style);
                    }
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    public static void HelpBox(string text, MessageType type, bool smallIcon = false)
    {
        Vector2 iconSize = EditorGUIUtility.GetIconSize();
        if (smallIcon) iconSize = new Vector2(20, 20);
        using (new EditorGUIUtility.IconSizeScope(iconSize))
        {
            GUIContent iconWText = EditorGUIUtility.TrTextContentWithIcon(text, type);
            GUIStyle labelStyle = EditorStyles.helpBox;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.richText = true;
            EditorGUILayout.LabelField(new GUIContent(), iconWText, labelStyle);
        }
    }

    public static bool DrawFoldoutHeader(string title, bool state, float height = 20f, string icon = null, bool miniLabel = false, bool hoverable = true)
    {
        GUIContent content = EditorGUIUtility.TrTextContent(" " + title, icon);
        Rect rect = GUILayoutUtility.GetRect(1f, height + EditorGUIUtility.standardVerticalSpacing);
        state = DrawFoldoutHeader(rect, content, state, miniLabel, hoverable);
        return state;
    }

    /// <summary>
    /// Create new GUI style with other property
    /// </summary>
    public static GUIStyle CreateGUIStyle(int fontSize = 30, FontStyle fontStyle = FontStyle.Bold,
        float fixedHeight = 40f, bool stretchWidth = true, TextAnchor textAnchor = TextAnchor.MiddleLeft)
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.fontStyle = fontStyle;
        style.fixedHeight = fixedHeight;
        style.stretchWidth = stretchWidth;
        style.alignment = textAnchor;
        style.normal.textColor = Color.white;
        return style;
    }

    public static bool DrawPropertyToggle(string label, bool state, float height = 20f, float width = 20f)
    {
        Rect rect = GUILayoutUtility.GetRect(1f, height);
        rect.xMin += width;
        return EditorGUI.Toggle(rect, label, state);
    }

    public static int DrawPropertyPopup(string label, int index, string[] displayedOptions, float height = 20f, float width = 20f)
    {
        Rect rect = EditorGUILayout.GetControlRect();
        rect.xMin += width;

        EditorGUI.PrefixLabel(rect, new GUIContent(label));
        rect.x += EditorGUIUtility.labelWidth;
        rect.width -= EditorGUIUtility.labelWidth;
        return EditorGUI.Popup(rect, index, displayedOptions);
    }

    public static float DrawPropertySlider(SerializedProperty property, string label, float minValue, float maxValue, string tooltip = null, float width = 20f)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = GUILayoutUtility.GetRect(1f, height);
        rect.xMin += width;
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        return (float)Math.Round(EditorGUI.Slider(rect, new GUIContent(label, tooltip), property.floatValue, minValue, maxValue), 2);
    }

    public static void DrawPropertyTextField(SerializedProperty property, float width)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = GUILayoutUtility.GetRect(1f, height);
        rect.xMin += width;
        EditorGUI.TextField(rect, property.ToString());
        EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
    }

    private static bool DrawFoldoutHeader(Rect rect, GUIContent content, bool hoverable, bool state)
    {
        Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0f);

        var foldoutRect = rect;
        foldoutRect.y += 4f;
        foldoutRect.x += 2f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;

        var labelRect = rect;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        // events
        var e = Event.current;
        if (rect.Contains(e.mousePosition))
        {
            if (hoverable) headerColor = new Color(0.6f, 0.6f, 0.6f, 0.2f);

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                state = !state;
                e.Use();
            }
        }

        EditorGUI.DrawRect(rect, headerColor);

        EditorGUIUtility.SetIconSize(new Vector2(15, 15));
        EditorGUI.LabelField(labelRect, content, EditorStyles.boldLabel);

        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);
        return state;
    }

    private static bool DrawFoldoutHeader(Rect rect, GUIContent content, bool state, bool miniLabel, bool hoverable)
    {
        Color headerColor = new Color(0.1f, 0.1f, 0.1f, 0f);

        var foldoutRect = rect;
        foldoutRect.y += 4f;
        foldoutRect.x += 2f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;

        var labelRect = rect;
        labelRect.y += miniLabel ? EditorGUIUtility.standardVerticalSpacing : 0f;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        // events
        var e = Event.current;
        if (rect.Contains(e.mousePosition))
        {
            if (hoverable) headerColor = new Color(0.6f, 0.6f, 0.6f, 0.2f);

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                state = !state;
                e.Use();
            }
        }

        EditorGUI.DrawRect(rect, headerColor);

        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        EditorGUI.LabelField(labelRect, content, miniLabel ? EditorStyles.miniBoldLabel : EditorStyles.boldLabel);

        return state;
    }

    private static void DrawRelativeProperties(SerializedProperty root, float width = 20f, string label = null, GUIStyle style = null, int space = 3)
    {
        if (label != null && style != null)
        {
            EditorGUILayout.Space(space);
            Rect newRect = EditorGUILayout.GetControlRect();
            newRect.xMin += 20f;
            EditorGUI.LabelField(newRect, label, style);
        }

        var childrens = root.GetVisibleChildrens();

        foreach (var childProperty in childrens)
        {
            float height = EditorGUI.GetPropertyHeight(childProperty, true);
            Rect rect = GUILayoutUtility.GetRect(1f, height);
            rect.xMin += width;
            EditorGUI.PropertyField(rect, childProperty, true);
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
    }

    private static void DrawOutline(Rect rect, RectOffset border)
    {
        Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        if (EditorGUIUtility.isProSkin)
        {
            color.r = 0.12f;
            color.g = 0.12f;
            color.b = 0.12f;
        }

        if (Event.current.type != EventType.Repaint)
            return;

        Color orgColor = GUI.color;
        GUI.color *= color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, border.top), EditorGUIUtility.whiteTexture); //top
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - border.bottom, rect.width, border.bottom), EditorGUIUtility.whiteTexture); //bottom
        GUI.DrawTexture(new Rect(rect.x, rect.y + 1, border.left, rect.height - 2 * border.left), EditorGUIUtility.whiteTexture); //left
        GUI.DrawTexture(new Rect(rect.xMax - border.right, rect.y + 1, border.right, rect.height - 2 * border.right), EditorGUIUtility.whiteTexture); //right

        GUI.color = orgColor;
    }

    private static void DrawOutline(Rect rect, RectOffset border, Color color)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        Color orgColor = GUI.color;
        GUI.color *= color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, border.top), EditorGUIUtility.whiteTexture); //top
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - border.bottom, rect.width, border.bottom), EditorGUIUtility.whiteTexture); //bottom
        GUI.DrawTexture(new Rect(rect.x, rect.y + 1, border.left, rect.height - 2 * border.left), EditorGUIUtility.whiteTexture); //left
        GUI.DrawTexture(new Rect(rect.xMax - border.right, rect.y + 1, border.right, rect.height - 2 * border.right), EditorGUIUtility.whiteTexture); //right

        GUI.color = orgColor;
    }

    private static Rect DrawHeaderWithBorder(string title, float height, ref Rect rect, bool rounded)
    {
        GUI.Box(rect, GUIContent.none, new GUIStyle(rounded ? "HelpBox" : "Tooltip"));
        rect.x += 1;
        rect.y += 1;
        rect.height -= 1;
        rect.width -= 2;

        var headerRect = rect;
        headerRect.height = height + EditorGUIUtility.standardVerticalSpacing;

        rect.y += headerRect.height;
        rect.height -= headerRect.height;

        EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.4f));

        var labelRect = headerRect;
        labelRect.y += EditorGUIUtility.standardVerticalSpacing;
        labelRect.x += 2f;

        EditorGUI.LabelField(labelRect, title, EditorStyles.miniBoldLabel);

        return headerRect;
    }

    private static Rect DrawHeaderWithBorder(string title, float height, ref Rect rect, GUIStyle boxStyle)
    {
        GUI.Box(rect, GUIContent.none, boxStyle);
        rect.x += 1;
        rect.y += 1;
        rect.height -= 1;
        rect.width -= 2;

        var headerRect = rect;
        headerRect.height = height + EditorGUIUtility.standardVerticalSpacing;

        rect.y += headerRect.height;
        rect.height -= headerRect.height;

        EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.4f));

        var labelRect = headerRect;
        labelRect.y += EditorGUIUtility.standardVerticalSpacing;
        labelRect.x += 2f;

        EditorGUI.LabelField(labelRect, title, EditorStyles.miniBoldLabel);

        return headerRect;
    }

    private static Rect DrawHeaderWithBorder(string title, float height, ref Rect rect, RectOffset border)
    {
        DrawOutline(rect, border);
        rect.x += 1;
        rect.y += 1;
        rect.height -= 1;
        rect.width -= 2;

        var headerRect = rect;
        headerRect.height = height + EditorGUIUtility.standardVerticalSpacing;

        rect.y += headerRect.height;
        rect.height -= headerRect.height;

        EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f, 0.4f));

        var labelRect = headerRect;
        labelRect.y += EditorGUIUtility.standardVerticalSpacing;
        labelRect.x += 2f;

        EditorGUI.LabelField(labelRect, title, EditorStyles.miniBoldLabel);

        return headerRect;
    }

    private static void TrHelpIconText(string message, string icon, bool rich = false)
    {
        GUIStyle style = new GUIStyle(EditorStyles.helpBox)
        {
            richText = rich
        };

        EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(" " + message, icon), style, new GUILayoutOption[0]);
    }

    private static void TrHelpIconText(Rect rect, string message, string icon, bool rich = false)
    {
        GUIStyle style = new GUIStyle(EditorStyles.helpBox)
        {
            richText = rich
        };

        EditorGUI.LabelField(rect, GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(" " + message, icon), style);
    }

    private static void TrHelpIconText(string message, MessageType messageType, bool rich = false, bool space = true)
    {
        string icon = string.Empty;

        GUIStyle style = new GUIStyle(EditorStyles.helpBox)
        {
            richText = rich
        };

        switch (messageType)
        {
            case MessageType.Info:
                icon = "console.infoicon.sml";
                break;
            case MessageType.Warning:
                icon = "console.warnicon.sml";
                break;
            case MessageType.Error:
                icon = "console.erroricon.sml";
                break;
        }

        if (!string.IsNullOrEmpty(icon))
        {
            string text = space ? " " + message : message;
            EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(text, icon), style, new GUILayoutOption[0]);
        }
        else
        {
            EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContent(message), style, new GUILayoutOption[0]);
        }
    }

    private static void TrHelpIconText(Rect rect, string message, MessageType messageType, bool rich = false, bool space = true)
    {
        string icon = string.Empty;

        GUIStyle style = new GUIStyle(EditorStyles.helpBox)
        {
            richText = rich
        };

        switch (messageType)
        {
            case MessageType.Info:
                icon = "console.infoicon.sml";
                break;
            case MessageType.Warning:
                icon = "console.warnicon.sml";
                break;
            case MessageType.Error:
                icon = "console.erroricon.sml";
                break;
        }

        if (!string.IsNullOrEmpty(icon))
        {
            string text = space ? " " + message : message;
            EditorGUI.LabelField(rect, GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(text, icon), style);
        }
        else
        {
            EditorGUI.LabelField(rect, GUIContent.none, EditorGUIUtility.TrTextContent(message), style);
        }
    }

    private static void TrIconText(string message, string icon, GUIStyle style, bool rich = false, bool space = true)
    {
        style.richText = rich;
        string text = space ? " " + message : message;
        EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(text, icon), style, new GUILayoutOption[0]);
    }

    private static void TrIconText(string message, MessageType messageType, GUIStyle style, bool rich = false, bool space = true)
    {
        string icon = string.Empty;
        style.richText = rich;

        switch (messageType)
        {
            case MessageType.Info:
                icon = "console.infoicon.sml";
                break;
            case MessageType.Warning:
                icon = "console.warnicon.sml";
                break;
            case MessageType.Error:
                icon = "console.erroricon.sml";
                break;
        }

        if (!string.IsNullOrEmpty(icon))
        {
            string text = space ? " " + message : message;
            EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(text, icon), style, new GUILayoutOption[0]);
        }
        else
        {
            EditorGUILayout.LabelField(GUIContent.none, EditorGUIUtility.TrTextContent(message), style, new GUILayoutOption[0]);
        }
    }

    private static void TrIconText(Rect rect, string message, MessageType messageType, GUIStyle style, bool rich = false, bool space = true)
    {
        string icon = string.Empty;
        style.richText = rich;

        switch (messageType)
        {
            case MessageType.Info:
                icon = "console.infoicon.sml";
                break;
            case MessageType.Warning:
                icon = "console.warnicon.sml";
                break;
            case MessageType.Error:
                icon = "console.erroricon.sml";
                break;
        }

        if (!string.IsNullOrEmpty(icon))
        {
            string text = space ? " " + message : message;
            EditorGUI.LabelField(rect, GUIContent.none, EditorGUIUtility.TrTextContentWithIcon(text, icon), style);
        }
        else
        {
            EditorGUI.LabelField(rect, GUIContent.none, EditorGUIUtility.TrTextContent(message), style);
        }
    }

    private static IEnumerable<SerializedProperty> GetVisibleChildrens(this SerializedProperty serializedProperty)
    {
        SerializedProperty currentProperty = serializedProperty.Copy();
        SerializedProperty nextSiblingProperty = serializedProperty.Copy();
        {
            nextSiblingProperty.NextVisible(false);
        }

        if (currentProperty.NextVisible(true))
        {
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;

                yield return currentProperty;
            }
            while (currentProperty.NextVisible(false));
        }
    }
}