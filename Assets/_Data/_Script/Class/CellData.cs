
[System.Serializable]
public class CellData
{
    public string spriteName;
    public int status;

    public CellData() { }
}
[System.Serializable]
public class Cells
{
    public CellData[] arrData;
}
