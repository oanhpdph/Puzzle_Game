using UnityEngine;

public class GameReset : MonoBehaviour
{
    private ISaveLoad saveLoad = new SaveLoad();

    private void Start()
    {
        ResetData();
    }
    public void ResetData()
    {
        saveLoad.Save("ScoreData.json", GameController.Instance.scoreData.ResetData());
        saveLoad.ClearFile("CellData.json");

    }
}
