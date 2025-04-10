using UnityEditor;
using UnityEngine;

public class AllGuiStylesExamples : EditorWindow
{
    private Vector2 _contentScroll;

    [MenuItem("Tools/All Gui Styles")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(AllGuiStylesExamples));
        window.titleContent = new GUIContent("All GUI Styles");
        window.Show();
    }

    private void OnGUI()
    {
        _contentScroll = EditorGUILayout.BeginScrollView(_contentScroll);

        var width = EditorGUILayout.GetControlRect(GUILayout.Height(0f)).width;
        foreach (var style in typeof(EditorStyles).GetProperties())
        {
            if (style.GetValue(null) is GUIStyle guistyle)
            {
                var content = new GUIContent(style.Name);
                var rect = EditorGUILayout.GetControlRect(false, guistyle.CalcHeight(content, width), guistyle);
                GUI.Box(rect, content, guistyle);
            }
        }

        EditorGUILayout.EndScrollView();
    }
}