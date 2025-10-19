using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : Panel
{
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI highScoreTxt;
    [SerializeField] private Button playBtn;

    private void OnEnable()
    {
        scoreTxt.text = GameController.Instance.ScoreData.currentScore.ToString();
        highScoreTxt.text = GameController.Instance.ScoreData.goalScore.ToString();
        playBtn.AddListener<object>(_ => PlayAction(), Listener.OnClick);
        SaveController.ClearGameData();
    }
    private void PlayAction()
    {
        GameController.Instance.ReLoadData();
        SceneManager.LoadScene(1);
        HUDSystem.Instance.Hide<EndPanel>();
    }
}
