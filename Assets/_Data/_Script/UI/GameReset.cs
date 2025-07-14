using UnityEngine;

public class GameReset : MonoBehaviour
{
    private ISaveLoad saveLoad = new SaveLoad();

    public void ResetData()
    {
        saveLoad.Save("ScoreData.json", UI_Score.scoreData.ResetData());
        saveLoad.ClearFile("CellData.json");

    }
}
