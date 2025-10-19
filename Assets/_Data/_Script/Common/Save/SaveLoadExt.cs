using System.Collections;
using UnityEngine;


public static class SaveLoadExt
{
    private static ISaveLoad saveLoad = new SaveLoad();

    public static void Save(string fileName, object data) => saveLoad.Save(fileName, data);
    public static T LoadData<T>(string fileName) => saveLoad.LoadData<T>(fileName);
    public static void ClearFile(string fileName) => saveLoad.ClearFile(fileName);


}
