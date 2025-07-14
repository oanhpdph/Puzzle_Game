using System.Collections;
using TMPro;
using UnityEngine;


public class UI_Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goalScoreUGUI;
    [SerializeField] private TextMeshProUGUI currentScoreUGUI;
    [SerializeField] private GameObject highScoreNoti;

    [HideInInspector]
    public static ScoreData scoreData;
    public GameObject ScorePrefab;
    private Canvas canvas;
    private ISaveLoad saveLoad = new SaveLoad();

    private void Awake()
    {
        scoreData = saveLoad.LoadData<ScoreData>("ScoreData.json");
        scoreData ??= new()
        {
            currentScore = 0,
            goalScore = 0,
            isHighScore = true
        };
    }
    private void Start()
    {
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
        yield return new WaitForSeconds(1.5f);
        highScoreNoti.SetActive(false);
    }
    public void AddScore(float score, Vector2 worldPosition)
    {
        GameObject popup = Instantiate(ScorePrefab, canvas.transform);

        popup.transform.position = worldPosition;
        popup.GetComponent<TextMeshProUGUI>().text = score.ToString();
        scoreData.currentScore += score;
        DisplayScore();
    }
}
