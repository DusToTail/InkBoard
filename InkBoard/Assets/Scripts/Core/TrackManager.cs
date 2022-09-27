using System.Collections.Generic;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public Track CurrentTrack { get { return m_CurrentTrack; } }

    private Track m_CurrentTrack;
    private List<Track> m_Tracks;

    private void Start()
    {
        m_Tracks = new List<Track>();
        m_CurrentTrack = null;

        string[] midFilePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.mid");
        string[] midiFilePaths = Directory.GetFiles(Application.streamingAssetsPath, "*.midi");
        List<string> paths = new List<string>();
        paths.AddRange(midFilePaths);
        paths.AddRange(midiFilePaths);
        foreach (string path in paths)
        {
            string fileName = Path.GetFileName(path);
            RegisterTrack(path, fileName.Split('.')[0]);
        }
        DebugLog();
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
    public bool RegisterTrack(string midiFilePath, string trackName)
    {
        var track = new Track(trackName);
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
    public Track(string trackName)
    {
        m_Name = trackName;
        m_Beats = new List<Beat>();
        m_BeatIndex = -1;
        m_CurrentTimeStamp = 0;


    }
    ~Track()
    {
    }

    public bool Init(string midiFilePath)
    {
        var midiFile = MidiFile.Read(midiFilePath);
        if (midiFile == null || midiFile.IsEmpty()) { return false; }

        var tempoMap = midiFile.GetTempoMap();
        var notes = midiFile.GetNotes();
        var notesArray = new Note[notes.Count];
        notes.CopyTo(notesArray, 0);
        for (int i = 0; i < notesArray.Length; i++)
        {
            var note = notesArray[i];
            var noteTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var beat = new Beat(noteTime.TotalMilliseconds, i);
            m_Beats.Add(beat);
        }
#if DEBUG
        DebugLog();
#endif
        return true;
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
    public double CurrentTimeStamp { get { return m_CurrentTimeStamp; } }
    public Beat CurrentBeat { get { return m_Beats[m_BeatIndex]; } }
    public Beat PreviousBeat { get { return m_Beats[m_BeatIndex - 1]; } }

    private double m_CurrentTimeStamp;
    private List<Beat> m_Beats;
    private int m_BeatIndex;
    private string m_Name;
}

public class Beat
{
    public Beat(double timeStampInMicroSeconds, int index)
    {
        m_TimeStamp = timeStampInMicroSeconds;
        m_Index = index;
    }
    ~Beat()
    {
    }
    public override string ToString()
    {
        var timeSpan = System.TimeSpan.FromMilliseconds(m_TimeStamp);
        return string.Format("Beat Index: {0}\nTime Stamp: {1}:{2}:{3}", m_Index, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }
    public double TimeStamp { get { return m_TimeStamp; } }
    public int Index { get { return m_Index; } }
    private double m_TimeStamp;
    private int m_Index;
}
