using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SpritePreviewAttribute resultAttribute = (SpritePreviewAttribute)attribute;
        return resultAttribute.Size;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var sprite = property.objectReferenceValue as Sprite;

        EditorGUI.BeginChangeCheck();
        Sprite newSprite = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Sprite), false) as Sprite;
        if (EditorGUI.EndChangeCheck())
        {
            property.objectReferenceValue = newSprite;
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }
}