using TMPro;
using UnityEngine;

public class UI_EndGame : MonoBehaviour
{
    public GameObject endGamePanel;

    public TextMeshProUGUI score;
    public TextMeshProUGUI goalScore;

    private void Start()
    {
        endGamePanel.SetActive(false);
    }
    public void EndGame()
    {
        endGamePanel.SetActive(true);
        score.text = UI_Score.scoreData.currentScore.ToString();
        goalScore.text = UI_Score.scoreData.goalScore.ToString();

    }
}
