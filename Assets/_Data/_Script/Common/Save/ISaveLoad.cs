
public interface ISaveLoad
{
    void Save(string fileName, object data);
    T LoadData<T>(string fileName);
    void ClearFile(string fileName);
}
