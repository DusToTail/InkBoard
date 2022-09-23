using UnityEngine;
using System.IO;

public static class SaveSystem<T> where T : class, IPersistentData
{
    public static void Save(string filePath, T fromObj)
    {
        if (!File.Exists(filePath)) { File.Create(filePath); }
        SaveObject saveObj = new SaveObject(filePath);
        fromObj.SaveTo(saveObj.Data);
        string content = JsonUtility.ToJson(saveObj.Data);
        FileSystem.WriteAll(filePath, content);
    }
    public static void AppendAndSave(string filePath, T fromObj)
    {
        if (!File.Exists(filePath)) { File.Create(filePath); }
        SaveObject saveObj = new SaveObject(filePath);
        fromObj.SaveTo(saveObj.Data);
        string content = JsonUtility.ToJson(saveObj.Data);
        FileSystem.AppendText(filePath, content);
    }
    public static void Load(string filePath, T toObj)
    {
        if (!File.Exists(filePath)) { Debug.LogWarning("FILE SYSTEM: " + filePath + " not found!"); toObj = null; }
        string content;
        FileSystem.ReadAll(filePath, out content);
        SaveObject saveObj = new SaveObject(filePath);
        saveObj.Data = JsonUtility.FromJson<T>(content);
        toObj.LoadFrom(saveObj.Data);
    }
    public static void LoadLine(string filePath, T toObj, uint lineIndex)
    {
        if (!File.Exists(filePath)) { Debug.LogWarning("FILE SYSTEM: " + filePath + " not found!"); toObj = null; }
        string content;
        FileSystem.ReadLine(filePath, lineIndex, out content);
        SaveObject saveObj = new SaveObject(filePath);
        saveObj.Data = JsonUtility.FromJson<T>(content);
        toObj.LoadFrom(saveObj.Data);
    }

    private class SaveObject
    {
        public SaveObject(string filePath, T obj)
        {
            FilePath = filePath;
            Data = obj;
        }
        public SaveObject(string filePath)
        {
            FilePath = filePath;
            Data = null;
        }
        ~SaveObject()
        {
        }
        public string FilePath { get; set; }
        public T Data { get; set; }
    }
}

public interface IPersistentData
{
    public void LoadFrom(object data);
    public void SaveTo(object data);
}

public static class FileSystem
{
    public static void AppendText(string filePath, string content)
    {
        File.AppendAllText(filePath, content);
    }
    public static void WriteAll(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public static void ReadAll(string filePath, out string content)
    {
        content = File.ReadAllText(filePath);
    }
    public static void ReadLines(string filePath, out string[] lines)
    {
        lines = File.ReadAllLines(filePath);
    }
    public static void ReadLine(string filePath, uint lineIndex, out string content)
    {
        var lines = File.ReadAllLines(filePath);
        if(lineIndex >= lines.Length) { content = string.Empty; }
        content = lines[lineIndex];
    }
}

