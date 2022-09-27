using System.Collections;
using UnityEngine;

public class RythmController : MonoBehaviour
{
    public delegate void BeatPlayed(Beat beat);
    public static event BeatPlayed OnBeatPlayed;

    public bool IsPlaying { get { return m_IsPlaying; } }
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private float playbackSpeed;
    [SerializeField] private bool isLooped;

    private bool m_IsPlaying = false;

    private Coroutine m_PlaybackCoroutine;

    public void StartPlay()
    {
        m_IsPlaying = true;
        if(m_PlaybackCoroutine != null)
        {
            StopCoroutine(m_PlaybackCoroutine);
            m_PlaybackCoroutine = null;
        }
        m_PlaybackCoroutine = StartCoroutine(PlaybackCoroutine());
    }
    public void StopPlay()
    {
        m_IsPlaying = false;
        if (m_PlaybackCoroutine != null)
        {
            StopCoroutine(m_PlaybackCoroutine);
            m_PlaybackCoroutine = null;
        }
        var track = trackManager.CurrentTrack;
        track.Reset();
        track.Source.Stop();
    }
#if DEBUG
    public void DebugLog()
    {
        Debug.Log("******** RYTHM CONTROLLER DEBUG *******");
        if(m_IsPlaying)
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
    private IEnumerator PlaybackCoroutine()
    {
        var track = trackManager.CurrentTrack;
        track.Reset();
        // How to set playback speed of track
        track.Source.pitch = playbackSpeed;
        track.Source.Play();
        double timer = Time.timeAsDouble;
        while (!track.IsFinished)
        {
            yield return null;
            var deltaTimeAsDoubleInSeconds = Time.timeAsDouble - timer;
            timer += deltaTimeAsDoubleInSeconds;
            track.SetCurrentTimeStamp(track.CurrentTimeStampInMilliSeconds + (deltaTimeAsDoubleInSeconds * 1000) * playbackSpeed);
            if (track.CurrentTimeStampInMilliSeconds >= track.CurrentBeat.TimeStampInMilliseconds + track.CurrentBeat.DurationInMilliseconds)
            {
                track.IncrementBeatIndex();
                if(OnBeatPlayed != null)
                    OnBeatPlayed(track.CurrentBeat);
            }
        }
        track.Source.Stop();

        if (isLooped)
            StartPlay();
    }
}


