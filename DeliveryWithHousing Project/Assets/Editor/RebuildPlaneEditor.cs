using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RebuildPlane))]
public class RebuildPlaneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Reset Plane"))
        {
            RebuildPlane.ResetPlane(Selection.activeGameObject.GetComponent<RebuildPlane>().MF.sharedMesh);
        }
    }
}
