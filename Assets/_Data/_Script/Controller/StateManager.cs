using TMPro;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI goalScore;
    [SerializeField] private GameObject panelEndGame;
    [SerializeField] private GameObject panelPause;

    private void Start()
    {
        panelEndGame.SetActive(false);
        panelPause.SetActive(false);
    }
    private void OnEnable()
    {
        GameController.Instance.OnChangeState += ChangeState;

    }
    private void OnDisable()
    {
        GameController.Instance.OnChangeState -= ChangeState;

    }
    public void ChangeState(StateGame stateGame)
    {
        switch (GameController.Instance.currentState)
        {
            case StateGame.pause:
                ShowPausePanel();
                break;
            case StateGame.play:
                HidePausePanel();
                break;
            case StateGame.end:
                ShowEndGame();
                break;
        }
    }
    public void ShowEndGame()
    {
        panelEndGame.SetActive(true);
        score.text = GameController.Instance.scoreData.currentScore.ToString();
        goalScore.text = GameController.Instance.scoreData.goalScore.ToString();

    }
    public void ShowPausePanel()
    {
        panelPause.SetActive(true);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetOptionScreenClip());
    }
    public void HidePausePanel()
    {
        panelPause.SetActive(false);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());
    }
}
