using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(OffsetCoord))]
public class OffsetCoordDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var lWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("Row")).x;

        // Calculate rects
        Rect colRect = new Rect(position.x, position.y, position.width / 2 - 10, position.height);
        Rect rowRect = new Rect(position.x + colRect.width + 5, position.y, colRect.width, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(colRect, property.FindPropertyRelative("col"));
        EditorGUI.PropertyField(rowRect, property.FindPropertyRelative("row"));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUIUtility.labelWidth = lWidth;

        EditorGUI.EndProperty();
    }
}