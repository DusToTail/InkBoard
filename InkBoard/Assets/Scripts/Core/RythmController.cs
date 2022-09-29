using System.Collections;
using UnityEngine;

public class RythmController : MonoBehaviour
{
    public delegate void BeatPlayed(Beat current);
    public event BeatPlayed OnBeatPlayed;

    public float PlaybackTimeStamp { get { return m_PlaybackTimeStamp; } }
    public float PlaybackSpeed { get { return playbackSpeed; } }
    public bool IsPlaying { get { return m_IsPlaying; } }
    public TimeLayout BeatLayout { get; private set; }

    [SerializeField] private TrackManager trackManager;
    [SerializeField] private float playbackSpeed;
    [SerializeField] private bool isLooped;
    [Range(0f, 1f)]
    [SerializeField] private float normalizedDelay;

    private bool m_IsPlaying = false;
    private float m_PlaybackTimeStamp = 0;
    private Coroutine m_PlaybackCoroutine;

    private void Start()
    {
        BeatLayout = new TimeLayout("BeatLayout");
        BeatLayout.PushBack("Input", 0.4f);
        BeatLayout.PushBack("Calculation", 0.05f);
        BeatLayout.PushBack("Execution", 0.5f);
        BeatLayout.PushBack("Clean", 0.05f);
    }

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
        StopAllCoroutines();
        m_PlaybackCoroutine = null;
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
        StartCoroutine(PlayTrackCoroutine(normalizedDelay * (float)track.CurrentBeat.DurationInMilliseconds / (playbackSpeed * 1000)));
        double timer = Time.timeAsDouble;
        m_PlaybackTimeStamp = (float)timer;
        while (!track.IsFinished)
        {
            var beatDurationInSeconds = track.CurrentBeat.DurationInMilliseconds / (playbackSpeed * 1000);

            if(track.CurrentBeat == track.PreviousBeat) { yield break; }
                
            if (OnBeatPlayed != null)
                OnBeatPlayed(track.CurrentBeat);

            yield return new WaitForSeconds((float)beatDurationInSeconds);
            // playback speed is not applied here to preserve its internal data
            track.SetCurrentTimeStamp(track.PlaybackTimeStampInMilliSeconds + track.CurrentBeat.DurationInMilliseconds);
            track.IncrementBeatIndex();
        }

        if (isLooped)
            StartPlay();
    }
    private IEnumerator PlayTrackCoroutine(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        var track = trackManager.CurrentTrack;
        track.Source.pitch = playbackSpeed;
        track.Source.Play();
        yield return new WaitForSeconds((float)track.DurationInMilliSeconds / (playbackSpeed * 1000));
        track.Source.Stop();
    }
}


