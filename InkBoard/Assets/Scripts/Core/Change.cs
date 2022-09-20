using System;
using System.Collections;
using System.Collections.Generic;

public class ChangeLayerStack
{
    public ChangeLayerStack()
    {
        m_List = new List<ChangeLayer>();
    }
    ~ChangeLayerStack()
    {
    }

    public virtual void Clear()
    {
        m_List.Clear();
    }
    public virtual void Add(ChangeLayer layer)
    {
        m_List.Add(layer);
    }
    public virtual void Remove(ChangeLayer layer)
    {
        m_List.Remove(layer);
    }
    public virtual void ForEach(Action<ChangeLayer> action)
    {
        for (int i = 0; i < m_List.Count; i++)
            action(m_List[i]);
    }

    private List<ChangeLayer> m_List;
}

public class ChangeLayer
{
    public ChangeLayer(string name)
    {
        m_List = new List<Change>();
        m_List.Capacity = 20;
        m_Name = name;
    }
    ~ChangeLayer()
    {
    }

    public virtual void Clear()
    {
        m_List.Clear();
    }
    public virtual void Add(Change change)
    {
        m_List.Add(change);
    }
    public virtual void Remove(Change change)
    {
        m_List.Remove(change);
    }
    public virtual void ForEach(Action<Change> action)
    {
        for (int i = 0; i < m_List.Count; i++)
            action(m_List[i]);
    }
    public override string ToString() { return m_Name; }

    private List<Change> m_List;
    private string m_Name;
}

public class Change
{
    public Change(string changeName)
    {
        m_Name = changeName;
    }
    ~Change()
    {
    }
    public virtual void Apply()
    {
        // Read and write to current game state
    }
    public virtual void Simulate()
    {
        // Read and write to simulating game state
    }
    public override string ToString() { return m_Name; }
    private string m_Name;
}
