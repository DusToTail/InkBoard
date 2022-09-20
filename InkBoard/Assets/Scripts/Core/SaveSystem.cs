using UnityEngine;
using System.IO;

public static class SaveSystem<T> where T : class, IPersistentData
{
    public static void Save(string filePath, T obj)
    {
        if (!File.Exists(filePath)) { File.Create(filePath); }
        SaveData<T> data = new SaveData<T>(filePath, obj);
        obj.SaveTo(data);
        string content = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, content);
    }

    public static void Load(string filePath, T obj)
    {
        if (!File.Exists(filePath)) { Debug.LogWarning("FILE SYSTEM: " + filePath + " not found!"); obj = null; }
        string content;
        FileSystem<T>.Read(filePath, out content);
        SaveData<T> data = JsonUtility.FromJson<SaveData<T>>(content);
        obj.LoadFrom(data);
    }
}

public static class FileSystem<T> where T : class, IPersistentData
{
    public static void Write(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public static void Read(string filePath, out string content)
    {
        content = File.ReadAllText(filePath);
    }
}

public class SaveData<T> where T : class, IPersistentData
{
    public SaveData(string filePath, T obj)
    {
        FilePath = filePath;
        Object = obj;
    }
    ~SaveData()
    {
    }
    public string FilePath { get; set; }
    public T Object { get; set; }
}

public interface IPersistentData
{
    public void LoadFrom(object data);
    public void SaveTo(object data);
}