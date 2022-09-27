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
            Debug.Log(track);
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
        m_CurrentTimeStampInMilliSeconds = 0;
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
            var beat = new Beat(noteTime.TotalMilliseconds, noteDuration.TotalMilliseconds, i);
            m_Beats.Add(beat);
        }
        return true;
    }
    public void Reset()
    {
        m_BeatIndex = 0;
        m_CurrentTimeStampInMilliSeconds = 0;
    }
    public void SetCurrentTimeStamp(double time)
    {
        m_CurrentTimeStampInMilliSeconds = time;
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
        foreach (var beat in m_Beats)
            Debug.Log(beat);
    }
#endif
    public override string ToString()
    {
        return string.Format("Name: {0}\nBeat Count: {1}", m_Name, m_Beats.Count);
    }
    public string Name { get { return m_Name; } }
    public double DurationInMilliSeconds { get { return m_DurationInMilliSeconds; } }
    public double CurrentTimeStampInMilliSeconds { get { return m_CurrentTimeStampInMilliSeconds; } }
    public Beat CurrentBeat { get { return m_Beats[m_BeatIndex]; } }
    public Beat PreviousBeat { get { return m_BeatIndex > 0 ? null : m_Beats[m_BeatIndex - 1]; } }
    public bool IsFinished { get { return m_CurrentTimeStampInMilliSeconds >= m_DurationInMilliSeconds; } }
    public AudioSource Source { get { return m_AudioSource; } }

    private double m_CurrentTimeStampInMilliSeconds;
    private List<Beat> m_Beats;
    private int m_BeatIndex;
    private string m_Name;
    private double m_DurationInMilliSeconds;
    private AudioSource m_AudioSource;
}

public class Beat
{
    public Beat(double timeStampInMilliSeconds, double durationInMilliSeconds, int index)
    {
        m_TimeStampInMilliseconds = timeStampInMilliSeconds;
        m_DurationInMilliseconds = durationInMilliSeconds;
        m_Index = index;
    }
    ~Beat()
    {
    }
    public override string ToString()
    {
        var startTimeStamp = System.TimeSpan.FromMilliseconds(m_TimeStampInMilliseconds);
        var duration = System.TimeSpan.FromMilliseconds(m_DurationInMilliseconds);
        var endTimeStamp = System.TimeSpan.FromMilliseconds(m_TimeStampInMilliseconds + m_DurationInMilliseconds);

        string startTimeStampString = string.Format("{0}:{1}:{2}", startTimeStamp.Minutes, startTimeStamp.Seconds, startTimeStamp.Milliseconds);
        string durationString = string.Format("{0}:{1}:{2}", duration.Minutes, duration.Seconds, duration.Milliseconds);
        string endTimeStampString = string.Format("{0}:{1}:{2}", endTimeStamp.Minutes, endTimeStamp.Seconds, endTimeStamp.Milliseconds);

        return string.Format("Beat Index: {0}\nDuration: {1}\nTime Stamp: {2} -> {3}", m_Index, durationString, startTimeStampString, endTimeStampString);
    }
    public double TimeStampInMilliseconds { get { return m_TimeStampInMilliseconds; } }
    public double DurationInMilliseconds { get { return m_DurationInMilliseconds; } }
    public int Index { get { return m_Index; } }
    private double m_TimeStampInMilliseconds;
    private double m_DurationInMilliseconds;
    private int m_Index;
}
