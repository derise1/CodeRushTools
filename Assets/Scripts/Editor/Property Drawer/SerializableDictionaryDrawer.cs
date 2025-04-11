using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Начало свойства
        EditorGUI.BeginProperty(position, label, property);

        // Отображение заголовка
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Получение ключей и значений
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        SerializedProperty valuesProperty = property.FindPropertyRelative("values");

        if (keysProperty != null && valuesProperty != null)
        {
            // Проверка на совпадение длины массивов
            if (keysProperty.arraySize != valuesProperty.arraySize)
            {
                EditorGUI.HelpBox(position, "Keys and Values count mismatch!", MessageType.Error);
                return;
            }

            // Рисуем элементы словаря
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                Rect keyRect = new Rect(position.x, position.y + i * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
                Rect valueRect = new Rect(position.x + position.width / 2, position.y + i * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyRect, keysProperty.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUI.PropertyField(valueRect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);
            }

            // Добавление кнопок для управления элементами
            Rect buttonRect = new Rect(position.x, position.y + keysProperty.arraySize * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonRect, "Add Element"))
            {
                keysProperty.Arra
                keysProperty.arraySize++;
                valuesProperty.arraySize++;
            }
        }
        else
        {
            EditorGUI.LabelField(position, "Invalid SerializableDictionary");
        }

        // Завершение свойства
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
        return (keysProperty != null ? keysProperty.arraySize : 1) * EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;
    }
}