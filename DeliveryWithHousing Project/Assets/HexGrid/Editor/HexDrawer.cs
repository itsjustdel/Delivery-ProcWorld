using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Hex))]
public class HexDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.FindPropertyRelative("q").intValue + property.FindPropertyRelative("r").intValue + property.FindPropertyRelative("s").intValue == 0)
        {
            return base.GetPropertyHeight(property, label);
        }

        return base.GetPropertyHeight(property, label) * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var zero = true;
        if (property.FindPropertyRelative("q").intValue + property.FindPropertyRelative("r").intValue + property.FindPropertyRelative("s").intValue != 0)
        {
            position.height = position.height / 3;
            zero = false;
        }

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var lWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(new GUIContent("Q")).x;

        var part = (position.width) / 3 - 5;

        // Calculate rects
        Rect qRect = new Rect(position.x + 5, position.y, part, position.height);
        Rect rRect = new Rect(position.x + part + 10, position.y, part, position.height);
        Rect sRect = new Rect(position.x + part * 2 + 15, position.y, part, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(qRect, property.FindPropertyRelative("q"));
        EditorGUI.PropertyField(rRect, property.FindPropertyRelative("r"));
        EditorGUI.PropertyField(sRect, property.FindPropertyRelative("s"));

        if (!zero)
        {
            position.y += position.height;
            position.height *= 2;

            var butQWidth = EditorStyles.miniButtonLeft.CalcSize(new GUIContent("Q")).x;
            var butRWidth = EditorStyles.miniButtonMid.CalcSize(new GUIContent("R")).x;
            var butSWidth = EditorStyles.miniButtonRight.CalcSize(new GUIContent("S")).x;

            var helpRect = new Rect(position.x, position.y, position.width - butQWidth - butRWidth - butSWidth - 5, position.height);
            var butQRect = new Rect(position.x + position.width - butQWidth - butRWidth - butSWidth, position.y, butQWidth, position.height);
            var butRRect = new Rect(position.x + position.width - butRWidth - butSWidth, position.y, butRWidth, position.height);
            var butSRect = new Rect(position.x + position.width - butSWidth, position.y, butSWidth, position.height);

            EditorGUI.HelpBox(helpRect, "Coordinates must add to 0", MessageType.Error);
            if (GUI.Button(butQRect, "Q", EditorStyles.miniButtonLeft))
            {
                CalcQ(property);
            }
            if (GUI.Button(butRRect, "R", EditorStyles.miniButtonMid))
            {
                CalcR(property);
            }
            if (GUI.Button(butSRect, "S", EditorStyles.miniButtonRight))
            {
                CalcS(property);
            }
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUIUtility.labelWidth = lWidth;

        EditorGUI.EndProperty();
    }

    void CalcQ(object obj)
    {
        var prop = (SerializedProperty)obj;

        prop.FindPropertyRelative("q").intValue = -prop.FindPropertyRelative("r").intValue - prop.FindPropertyRelative("s").intValue;
    }

    void CalcR(object obj)
    {
        var prop = (SerializedProperty)obj;

        prop.FindPropertyRelative("r").intValue = -prop.FindPropertyRelative("q").intValue - prop.FindPropertyRelative("s").intValue;
    }

    void CalcS(object obj)
    {
        var prop = (SerializedProperty)obj;

        prop.FindPropertyRelative("s").intValue = -prop.FindPropertyRelative("q").intValue - prop.FindPropertyRelative("r").intValue;
    }
}