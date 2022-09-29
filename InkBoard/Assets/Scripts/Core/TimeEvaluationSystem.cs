using System.Collections.Generic;
using System;

public static class TimeEvaluation
{
    public static string Evaluate(TimeLayout layout, float timing, bool isNormalized)
    {
        if(layout == null || layout.Sections.Length == 0) { return string.Empty; }
        if(isNormalized)
            if (timing < 0 || timing > 1) { return string.Empty; }
        else
            if (timing < 0 || timing > layout.TotalDuration) { return string.Empty; }

        string result = layout.Sections[0].Name;
        for (int i = 1; i < layout.Sections.Length; i++)
        {
            string name = layout.Sections[i].Name;
            if (timing >= layout.GetStart(name, isNormalized))
                result = name;
        }
        return result;
    }
    

}

public class TimeLayout
{
    public TimeLayout(string name)
    {
        m_Sections = new List<Section>();
        TotalDuration = 0;
        Name = name;
    }
    ~TimeLayout()
    {
    }
    public float GetDurations(bool isNormalized = false, params string[] names)
    {
        float result = 0;
        foreach (string name in names)
        {
            var section = m_Sections.Find(x => x.Name == name);
            if (section == null) { continue; }
            result += section.Duration;
        }
        if (isNormalized)
            return result / TotalDuration;
        return result;
    }
    public float GetDuration(string name, bool isNormalized)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return 0; }
        if(isNormalized)
            return section.Duration / TotalDuration;
        return section.Duration;
    }
    public float GetStart(string name, bool isNormalized)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return 0; }
        if (isNormalized)
            return section.Start / TotalDuration;
        return section.Start;
    }
    public float GetEnd(string name, bool isNormalized)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return 0; }
        if (isNormalized)
            return section.End / TotalDuration;
        return section.End;
    }
    public float GetComparisonToTotal(float duration)
    {
        if (TotalDuration == 0) { return 0; }
        return duration / TotalDuration;
    }
    public Section[] GetSections(Predicate<Section> predicate)
    {
        var sections = m_Sections.FindAll(predicate);
        if (sections == null) { return null; }
        return sections.ToArray();
    }
    public Section GetSection(string name)
    {
        var section = m_Sections.Find(x => x.Name == name);
        if (section == null) { return null; }
        return section;
    }
    public void PushBack(string name, float duration)
    {
        m_Sections.Add(new Section(name, duration, TotalDuration));
        TotalDuration += duration;
    }

    public string Name { get; private set; }
    public float TotalDuration { get; private set; }
    public Section[] Sections { get { return m_Sections.ToArray(); } }
    private List<Section> m_Sections;

    public class Section
    {
        public Section(string name, float duration, float timeStamp)
        {
            Name = name;
            Duration = duration;
            Start = timeStamp;
        }
        ~Section()
        {
        }
        public string Name { get; set; }
        public float Duration { get; set; }
        public float Start { get; set; }
        public float End { get { return Start + Duration; } }
    }
}
