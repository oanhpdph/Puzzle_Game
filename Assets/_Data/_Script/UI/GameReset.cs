using Assets._Data._Script.Controller;
using UnityEngine;

public class GameReset : MonoBehaviour
{

    public void ResetData()
    {
        SaveController.Instance.ClearFile();
        PlayerPrefs.SetFloat("currentScore", 0);
        PlayerPrefs.SetFloat("goalScore", 0);
        PlayerPrefs.SetInt("isHigh", 0);

        for (int i = 1; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("Shape" + i);
        }
    }
}
