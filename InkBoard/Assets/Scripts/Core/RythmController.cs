using System.Collections;
using UnityEngine;

public class RythmController : MonoBehaviour
{
    public TrackManager trackManager;
    public float playbackSpeed;
    public bool isLooped;
    public float trackOffsetInSeconds;

    public bool IsPlaying { get; set; }
    public TimeLayout BeatLayout { get; private set; }

    private void Start()
    {
        BeatLayout = new TimeLayout("BeatLayout");
        BeatLayout.PushBack("Input", 0.4f);
        BeatLayout.PushBack("Calculation", 0.1f);
        BeatLayout.PushBack("Execution", 0.4f);
        BeatLayout.PushBack("Clean", 0.1f);
    }

#if DEBUG
    public void DebugLog()
    {
        if(IsPlaying)
        {
            string format = "Currently playing... \nTrack Name: {0}\nPlayback Speed: {1}\nIs Looped: {2}";
            string message = string.Format(format, trackManager.CurrentTrack.Name, playbackSpeed, isLooped);
            Debug.Log(message);
        }
        else
        {
            string format = "On standby... \nTrack Name: {0}\nPlayback Speed: {1}\nIs Looped: {2}";
            string message = string.Format(format, trackManager.CurrentTrack.Name, playbackSpeed, isLooped);
            Debug.Log(message);
        }
    }
#endif
    
}


