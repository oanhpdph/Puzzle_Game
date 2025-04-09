using System.IO;
using UnityEngine;


public class SaveController
{
    public static void Save(string fileName, object data)
    {
        Debug.Log(Application.persistentDataPath + "/" + fileName);

        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        string path = Application.persistentDataPath + "/" + fileName;

        System.IO.File.WriteAllText(path, json);
    }
    public static T LoadData<T>(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        Debug.LogWarning("Save file not found: " + path);
        return default;
    }

    public static void ClearFile(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        System.IO.File.WriteAllText(path, "");
    }
}
