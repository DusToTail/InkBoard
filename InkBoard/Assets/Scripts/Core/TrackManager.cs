using System.Collections.Generic;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] m_AudioSources;

    public Track CurrentTrack { get { return m_CurrentTrack; } }
    private Track m_CurrentTrack;
    private List<Track> m_Tracks;

    private void Start()
    {
        m_Tracks = new List<Track>();
        m_CurrentTrack = null;
    }
    public void Init()
    {
        List<string> paths = new List<string>();
        string[] midFilePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.mid");
        string[] midiFilePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.midi");
        paths.Clear();
        paths.AddRange(midFilePaths);
        paths.AddRange(midiFilePaths);
        foreach (string path in paths)
        {
            string fileName = Path.GetFileName(path);
            string trackName = fileName.Split('.')[0];
            RegisterTrack(path, trackName, System.Array.Find(m_AudioSources, (x) => x.clip.name == trackName));
        }
        SetCurrentTrack("default");


    }
    public bool SetCurrentTrack(string trackName)
    {
        var track = m_Tracks.Find(x => x.Name == trackName);
        if (track != null)
        {
            Debug.Log($"TrackManager: Set {trackName} current!");
            m_CurrentTrack = track;
            return true;
        }
        else
        {
            Debug.Log($"TrackManager: {trackName} does not exist!");
            return false;
        }
    }
    public bool RegisterTrack(string midiFilePath, string trackName, AudioSource source)
    {
        var track = new Track(trackName, source);
        bool success = track.Init(midiFilePath);
        if (success)
        {
            Debug.Log($"TrackManager: Registered {trackName} successfully!");
            m_Tracks.Add(track);
            return true;
        }
        else
        {
            Debug.Log($"TrackManager: Failed to registered {trackName}!");
            return false;
        }
    }
    public void Clear()
    {
        m_Tracks.Clear();
        m_CurrentTrack = null;
    }
#if DEBUG
    public void DebugLog()
    {
        Debug.Log("******* DEBUG TRACK MANAGER********");
        Debug.Log("Number of tracks: " + m_Tracks.Count);
        foreach (var track in m_Tracks)
            track.DebugLog();
    }
#endif
}

public class Track
{
    public Track(string trackName, AudioSource source)
    {
        m_Name = trackName;
        m_Beats = new List<Beat>();
        m_BeatIndex = 0;
        m_DurationInMilliSeconds = 0;
        m_AudioSource = source;
    }
    ~Track()
    {
    }

    public bool Init(string midiFilePath)
    {
        var midiFile = MidiFile.Read(midiFilePath);
        if (midiFile == null || midiFile.IsEmpty()) { return false; }
        var timeSpan = midiFile.GetDuration(TimeSpanType.Metric);
        var tempoMap = midiFile.GetTempoMap();
        m_DurationInMilliSeconds = TimeConverter.ConvertTo<MetricTimeSpan>(timeSpan, tempoMap).TotalMilliseconds;
        var notes = midiFile.GetNotes();
        var notesArray = new Note[notes.Count];
        notes.CopyTo(notesArray, 0);
        for (int i = 0; i < notesArray.Length; i++)
        {
            var note = notesArray[i];
            var noteTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var noteDuration = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);
            var beat = new Beat(noteTime.TotalMilliseconds, noteDuration.TotalMilliseconds, 0, i);
            // No previous beat during first iteration to set interval
            if(m_Beats.Count > 1)
            {
                var prevBeat = m_Beats[m_Beats.Count - 1];
                prevBeat.IntervalInMilliseconds = beat.TimeStampInMilliseconds - prevBeat.TimeStampInMilliseconds;
            }
            m_Beats.Add(beat);
            // No next beat during last iteration to set interval
            if (i == notesArray.Length - 1)
            {
                var lastBeat = m_Beats[m_Beats.Count - 1];
                lastBeat.IntervalInMilliseconds = m_DurationInMilliSeconds - beat.TimeStampInMilliseconds;
            }
        }
        return true;
    }
    public Beat GetBeat(int index)
    {
        if(index < 0 || index >= m_Beats.Count) { return null; }
        return m_Beats[index];
    }
    public Beat[] GetBeats()
    {
        return m_Beats.ToArray();
    }
    public void ResetIndex()
    {
        m_BeatIndex = 0;
    }
    public void IncrementBeatIndex()
    {
        m_BeatIndex++;
    }
    public void DecrementBeatIndex()
    {
        m_BeatIndex--;
    }
#if DEBUG
    public void DebugLog()
    {
        Debug.Log("******* DEBUG TRACK********");
        Debug.Log(this);
        string message = "";
        foreach (var beat in m_Beats)
            message += beat.ToString() + '\n';
        Debug.Log(message);
    }
#endif
    public override string ToString()
    {
        return string.Format("Name: {0}\nBeat Count: {1}", m_Name, m_Beats.Count);
    }
    public string Name { get { return m_Name; } }
    public double DurationInMilliSeconds { get { return m_DurationInMilliSeconds; } }
    public Beat CurrentBeat { get { return m_BeatIndex > m_Beats.Count - 1 ? m_Beats[m_Beats.Count - 1] : m_Beats[m_BeatIndex]; } }
    public Beat PreviousBeat { get { return m_BeatIndex < 1 ? null : m_Beats[m_BeatIndex - 1]; } }
    public AudioSource Source { get { return m_AudioSource; } }

    private List<Beat> m_Beats;
    private int m_BeatIndex;
    private string m_Name;
    private double m_DurationInMilliSeconds;
    private AudioSource m_AudioSource;
}

public class Beat
{
    public Beat(double timeStampInMilliSeconds, double durationInMilliSeconds, double intervalInMilliSeconds, int index)
    {
        TimeStampInMilliseconds = timeStampInMilliSeconds;
        DurationInMilliseconds = durationInMilliSeconds;
        IntervalInMilliseconds = intervalInMilliSeconds;
        m_Index = index;
    }
    ~Beat()
    {
    }
    public override string ToString()
    {
        var startTimeStamp = System.TimeSpan.FromMilliseconds(TimeStampInMilliseconds);
        var duration = System.TimeSpan.FromMilliseconds(DurationInMilliseconds);
        var interval = System.TimeSpan.FromMilliseconds(IntervalInMilliseconds);
        var endTimeStamp = System.TimeSpan.FromMilliseconds(TimeStampInMilliseconds + IntervalInMilliseconds);

        string startTimeStampString = string.Format("{0}:{1}:{2}", startTimeStamp.Minutes, startTimeStamp.Seconds, startTimeStamp.Milliseconds);
        string durationString = string.Format("{0}:{1}:{2}", duration.Minutes, duration.Seconds, duration.Milliseconds);
        string intervalString = string.Format("{0}:{1}:{2}", interval.Minutes, interval.Seconds, interval.Milliseconds);
        string endTimeStampString = string.Format("{0}:{1}:{2}", endTimeStamp.Minutes, endTimeStamp.Seconds, endTimeStamp.Milliseconds);

        return string.Format("Beat Index: {0}\nDuration: {1}\nInterval: {2}\nTime Stamp: {3} -> {4}", m_Index, durationString, intervalString, startTimeStampString, endTimeStampString);
    }
    public double TimeStampInMilliseconds { get; set; }
    public double DurationInMilliseconds { get; set; }
    public double IntervalInMilliseconds { get; set; }
    public int Index { get { return m_Index; } }
    private int m_Index;
}
