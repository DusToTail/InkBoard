using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackManager))]
public class TrackManagerEditor : Editor
{
    private string currentTrackName;
    public override void OnInspectorGUI()
    {
        TrackManager manager = (TrackManager)target;
        base.OnInspectorGUI();
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("***Editor***");
        if(GUILayout.Button("Init"))
        {
            manager.Init();
        }
        currentTrackName = GUILayout.TextField(currentTrackName);
        if (GUILayout.Button("SelectAboveAsCurrentTrack"))
        {
            manager.SetCurrentTrack(currentTrackName);
        }
#if DEBUG
        if (GUILayout.Button("DebugLog"))
        {
            manager.DebugLog();
        }
#endif
    }
}
