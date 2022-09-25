using UnityEngine;
using System.Collections.Generic;

public class InGameStateRecorder
{
    public InGameDataString[] AllDatas { get { return m_Datas.ToArray(); } }
    public InGameDataString Simulation { get { return m_SimulatedData; } }
    public InGameDataString Current { get { return AllDatas[m_CurrentIndex]; } }

    private List<InGameDataString> m_Datas;
    private InGameDataString m_SimulatedData;
    private int m_CurrentIndex;

    public InGameStateRecorder()
    {
        m_Datas = new List<InGameDataString>();
        m_SimulatedData = null;
        m_CurrentIndex = -1;
    }
    ~InGameStateRecorder()
    {
    }
    public void Begin()
    {

    }
    public void Log()
    {

    }
    public void End()
    {

    }

    [System.Serializable]
    public class InGameDataString : IPersistentData
    {
        public string boardData;
        public string charactersData;

        public void LoadFrom(object data)
        {
            InGameDataString inGameData = data as InGameDataString;
            boardData = inGameData.boardData;
            charactersData = inGameData.charactersData;
        }

        public void SaveTo(object data)
        {
            InGameDataString inGameData = data as InGameDataString;
            inGameData.boardData = boardData;
            inGameData.charactersData = charactersData;
        }
    }

}
public abstract class Change
{
    public abstract bool Execute();
    public abstract bool IsValid();
}
public abstract class Movement : Change
{
}