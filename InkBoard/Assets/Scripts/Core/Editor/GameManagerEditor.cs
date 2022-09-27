using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameManager gameManager = (GameManager)target;
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("***Editor***");
        if(GUILayout.Button("Init"))
        {
            gameManager.Init();
        }
    }
}
