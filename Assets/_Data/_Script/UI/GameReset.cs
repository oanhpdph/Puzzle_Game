using UnityEngine;

public class GameReset : MonoBehaviour
{
    private void Start()
    {
        ResetData();
    }
    public void ResetData()
    {
        SaveLoadExt.Save("ScoreData.json", GameController.Instance.ScoreData.ResetData());
        SaveLoadExt.ClearFile("CellData.json");
    }
}
