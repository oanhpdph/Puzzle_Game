using System.Collections;
using TMPro;
using UnityEngine;


public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalScoreUGUI;
    [SerializeField] private TextMeshProUGUI currentScoreUGUI;
    [SerializeField] private GameObject highScoreNotif;

    private Canvas canvas;
    public GameObject ScorePrefab;
    public GameObject ComboPrefab;
    private void Start()
    {
        ScorePrefab.CreatePool();
        ComboPrefab.CreatePool();
        highScoreNotif.CreatePool();
        GameController.Instance.ScoreData ??= new()
        {
            currentScore = 0,
            goalScore = 0,
            isHighScore = true,
            combo = 0
        };
        DisplayScore();
        canvas = GetComponentInParent<Canvas>();
    }

    public void DisplayScore()
    {
        ScoreData scoreData = GameController.Instance.ScoreData;

        goalScoreUGUI.text = scoreData.goalScore.ToString();
        currentScoreUGUI.text = scoreData.currentScore.ToString();
        if (scoreData.currentScore > scoreData.goalScore)
        {
            scoreData.goalScore = scoreData.currentScore;
            goalScoreUGUI.text = scoreData.currentScore.ToString();

            if (!scoreData.isHighScore)
            {
                ShowHighScoreNotification();
                scoreData.isHighScore = true;
            }
        }
    }
    private void ShowHighScoreNotification()
    {
        highScoreNotif.Use(1.5f, out var go);
        go.transform.SetParent(canvas.transform);
        go.transform.localScale = Vector3.one;
        AudioController.Instance.PlayOneShot(AudioAssets.Instance.GetWinClip());
    }
    public void AddScore(float score, Vector2 worldPosition, int combo)
    {
        ScorePrefab.Use(0.5f, out var go);
        go.transform.SetParent(canvas.transform);
        go.transform.localScale = Vector3.one;
        if (combo >= 2)
        {
            ComboPrefab.Use(0.5f, out var comboClone);
            comboClone.transform.SetParent(canvas.transform);
            comboClone.transform.localScale = Vector3.one;
            comboClone.transform.localPosition = Vector3.zero;
            comboClone.GetComponent<TextMeshProUGUI>().text = $"Combo x{combo}";
        }
        go.transform.position = worldPosition;
        go.GetComponent<TextMeshProUGUI>().text = $"+ {score}";
        GameController.Instance.ScoreData.currentScore += score;
        GameController.Instance.ScoreData.combo = combo;
        DisplayScore();
    }
}
