using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RythmController))]
public class RythmControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RythmController controller = (RythmController)target;
        base.OnInspectorGUI();
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("***Editor***");
#if DEBUG
        if (GUILayout.Button("DebugLog"))
        {
            controller.DebugLog();
        }
#endif
    }
}
