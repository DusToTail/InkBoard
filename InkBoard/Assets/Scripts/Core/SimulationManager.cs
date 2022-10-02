using System.Collections.Generic;

public class SimulationManager
{
    public SimulationManager()
    {
        m_Simulations = new List<Simulation>();
        m_Count = 0;
    }
    ~SimulationManager()
    {
    }
    public void Add(Simulation sim)
    {
        m_Simulations.Add(sim);
        sim.Index = m_Count;
        m_Count++;
    }
    public Simulation GetSimulation(int index)
    {
        if(index < 0 || index >= m_Count) { return null; }
        var sim = m_Simulations[index];
        return sim;
    }
    public Simulation GetSimulation(string ID)
    {
        var sim = m_Simulations.Find(x => x.ID == ID);
        return sim;
    }
    public Simulation[] Simulations { get { return m_Simulations.ToArray(); } }
    private List<Simulation> m_Simulations = new List<Simulation>();
    private int m_Count;
}

public class Simulation
{
    public Simulation(string id)
    {
        ID = id;
        Index = 0;
    }
    ~Simulation()
    {
    }
    public int Index { get; set; }
    public string ID { get; private set; }
}

public interface ISimulate<T> where T : Simulation
{
    void Simulate(T refSim);
}
