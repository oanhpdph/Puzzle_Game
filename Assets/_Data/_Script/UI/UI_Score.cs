using System.Collections;
using TMPro;
using UnityEngine;


public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalScoreUGUI;
    [SerializeField] private TextMeshProUGUI currentScoreUGUI;
    [SerializeField] private GameObject highScoreNoti;

    private ScoreData scoreData;
    private Canvas canvas;
    public GameObject ScorePrefab;
    public GameObject ComboPrefab;
    private void Start()
    {
        scoreData = GameController.Instance.scoreData;
        scoreData ??= new()
        {
            currentScore = 0,
            goalScore = 0,
            isHighScore = true
        };
        DisplayScore();
        canvas = GetComponentInParent<Canvas>();
    }

    public void DisplayScore()
    {
        goalScoreUGUI.text = scoreData.goalScore.ToString();
        currentScoreUGUI.text = scoreData.currentScore.ToString();

        if (scoreData.currentScore > scoreData.goalScore)
        {
            scoreData.goalScore = scoreData.currentScore;
            goalScoreUGUI.text = scoreData.currentScore.ToString();

            if (!scoreData.isHighScore)
            {
                StartCoroutine(ShowHighScoreNotification());
                scoreData.isHighScore = true;
            }
        }
    }
    private IEnumerator ShowHighScoreNotification()
    {
        highScoreNoti.SetActive(true);
        AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetWinClip());
        yield return new WaitForSeconds(1.5f);
        highScoreNoti.SetActive(false);
    }
    public void AddScore(float score, Vector2 worldPosition, int combo)
    {
        GameObject popup = Instantiate(ScorePrefab, canvas.transform);
        if (combo >= 2)
        {
            GameObject popupCombo = Instantiate(ComboPrefab, canvas.transform);
            popupCombo.GetComponent<TextMeshProUGUI>().text = $"Combo x{combo}";
        }
        popup.transform.position = worldPosition;
        popup.GetComponent<TextMeshProUGUI>().text = "+" + score.ToString();
        scoreData.currentScore += score;
        DisplayScore();
    }
}
