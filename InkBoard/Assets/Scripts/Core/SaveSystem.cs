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
        File.WriteAllText(filePath, content);
    }

    public static void Load(string filePath, T toObj)
    {
        if (!File.Exists(filePath)) { Debug.LogWarning("FILE SYSTEM: " + filePath + " not found!"); toObj = null; }
        string content;
        FileSystem.Read(filePath, out content);
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
    public static void Write(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    public static void Read(string filePath, out string content)
    {
        content = File.ReadAllText(filePath);
    }
}

