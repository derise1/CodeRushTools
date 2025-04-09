using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DatabaseViewAttribute))]
public class DatabaseViewDrawer : PropertyDrawer
{
    private const float ROW_HEIGHT = 20f;
    private const float HEADER_HEIGHT = 22f;
    private const float PADDING = 2f;
    private const float SCROLLBAR_WIDTH = 15f;

    private Vector2 scrollPos;
    private bool isExpanded = true;
    private FieldInfo[] elementFields;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!isExpanded) return EditorGUIUtility.singleLineHeight;
        
        int rows = property.isArray ? property.arraySize : 0;
        return EditorGUIUtility.singleLineHeight + // Header
               (rows > 0 ? HEADER_HEIGHT + (rows * ROW_HEIGHT) : ROW_HEIGHT) + // Content
               PADDING * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!property.isArray)
        {
            EditorGUI.HelpBox(position, "DatabaseView works only with arrays and lists!", MessageType.Error);
            return;
        }

        position.height = EditorGUIUtility.singleLineHeight;
        isExpanded = EditorGUI.Foldout(position, isExpanded, label, true);
        
        if (!isExpanded) return;

        position.y += EditorGUIUtility.singleLineHeight + PADDING;
        position.height = GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight - PADDING;

        if (elementFields == null)
            CacheElementFields(property);

        DrawDatabaseTable(position, property);
    }

    private void CacheElementFields(SerializedProperty property)
    {
        if (property.arraySize == 0) return;
        
        SerializedProperty firstElement = property.GetArrayElementAtIndex(0);
        System.Type elementType = GetTypeFromProperty(firstElement);
        elementFields = elementType?.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    private void DrawDatabaseTable(Rect position, SerializedProperty property)
    {
        if (elementFields == null || elementFields.Length == 0)
        {
            EditorGUI.HelpBox(position, "No public fields found in array elements!", MessageType.Info);
            return;
        }

        Rect tableRect = new Rect(position.x, position.y, position.width, position.height);
        Rect viewRect = new Rect(0, 0, 
            Mathf.Max(tableRect.width, GetTotalWidth(tableRect.width)), 
            tableRect.height);

        scrollPos = GUI.BeginScrollView(tableRect, scrollPos, viewRect);
        {
            DrawHeaders(viewRect.width);
            DrawRows(property);
        }
        GUI.EndScrollView();
    }

    private void DrawHeaders(float width)
    {
        if (elementFields == null) return;

        float columnWidth = width / elementFields.Length;
        Rect headerRect = new Rect(0, 0, columnWidth, HEADER_HEIGHT);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(5, 5, 3, 3)
        };

        foreach (var field in elementFields)
        {
            GUI.Label(headerRect, ObjectNames.NicifyVariableName(field.Name), headerStyle);
            headerRect.x += columnWidth;
        }
    }

    private void DrawRows(SerializedProperty property)
    {
        if (elementFields == null || property.arraySize == 0) return;

        float yPos = HEADER_HEIGHT;
        float columnWidth = Screen.width / elementFields.Length;

        GUIStyle cellStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(5, 5, 3, 3)
        };

        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            DrawRow(element, yPos, columnWidth, cellStyle);
            yPos += ROW_HEIGHT;
        }
    }
            
    private void DrawRow(SerializedProperty element, float yPos, float columnWidth, GUIStyle style)
    {
        Rect rowRect = new Rect(0, yPos, columnWidth, ROW_HEIGHT);
        object elementObj = GetTargetObject(element);

        foreach (var field in elementFields)
        {
            object value = field.GetValue(elementObj);
            GUI.Label(rowRect, value?.ToString() ?? "null", style);
            rowRect.x += columnWidth;
        }
    }

    private float GetTotalWidth(float availableWidth)
    {
        return elementFields?.Length > 0 
            ? elementFields.Length * (availableWidth / elementFields.Length)
            : availableWidth;
    }

    private System.Type GetTypeFromProperty(SerializedProperty prop)
    {
        return GetTargetObject(prop)?.GetType();
    }

    private object GetTargetObject(SerializedProperty prop)
    {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        return GetNestedObject(path, obj);
    }

    private object GetNestedObject(string path, object obj)
    {
        foreach (string part in path.Split('.'))
        {
            if (obj == null) return null;
            System.Type type = obj.GetType();
            FieldInfo field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return null;
            obj = field.GetValue(obj);
        }
        return obj;
    }
}