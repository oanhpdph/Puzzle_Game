using TMPro;
using UnityEngine;

public class UI_EndGame : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI goalScore;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void EndGame()
    {
        this.gameObject.SetActive(true);
        score.text = UI_Score.scoreData.currentScore.ToString();
        goalScore.text = UI_Score.scoreData.goalScore.ToString();

    }
}
