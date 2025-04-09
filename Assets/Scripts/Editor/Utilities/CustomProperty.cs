using UnityEditor;
using UnityEngine;

public sealed class CustomProperty
{
    public SerializedProperty Property { get; private set; }
    public string Tooltip { get; private set; }
    public string Label { get; private set; }
    public GUIStyle Style { get; private set; }

    public CustomProperty(SerializedProperty property, string toolTip = null, string label = null, GUIStyle style = null)
    {
        Property = property;
        Tooltip = toolTip;
        Label = label;
        Style = style;
    }
}