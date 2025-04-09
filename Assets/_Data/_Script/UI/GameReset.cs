using UnityEngine;

public class GameReset : MonoBehaviour
{

    public void ResetData()
    {
        SaveController.Save("ScoreData.json", UI_Score.scoreData.ResetData());
        SaveController.ClearFile("CellData.json");


        for (int i = 1; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("Shape" + i);
        }
    }
}
