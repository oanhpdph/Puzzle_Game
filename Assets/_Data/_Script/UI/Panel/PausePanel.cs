using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [Header("Music")]
    [SerializeField] private Image musicOn;
    [SerializeField] private Image musicOff;
    [SerializeField] private Button musicButton;
    private bool isMusicOn = true;
    [Header("Button")]
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button restart;
    [SerializeField] private Button closeBtn;
    private void Start()
    {
        isMusicOn = !PlayerData.Music;// true on false off
        MusicAction();
        musicButton.AddListener<object>(_ => MusicAction(), Listener.OnClick);
        homeBtn.AddListener<object>(_ => HomeAction(), Listener.OnClick);
        restart.AddListener<object>(_ => RestartAction(), Listener.OnClick);
        closeBtn.AddListener<object>(_ => CloseAction(), Listener.OnClick);

    }
    private void OnEnable()
    {
        isMusicOn = !PlayerData.Music;
        MusicAction();
        Time.timeScale = 0;
        //AudioController.Instance.PlayAudio(AudioAssets.Instance.GetOptionScreenClip());
    }
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
    private void MusicAction()
    {
        this.isMusicOn = !isMusicOn;
        AudioController.Instance.OnOffMusic(isMusicOn);
        PlayerData.Music = isMusicOn;
        SetUp(isMusicOn);
    }
    private void SetUp(bool isMusicOn)
    {
        musicOn.gameObject.SetActive(isMusicOn);
        musicOff.gameObject.SetActive(!isMusicOn);
    }
    private void HomeAction()
    {
        SaveController.SaveData();
        SceneManager.LoadScene(0);
        HUDSystem.Instance.Hide<PausePanel>();
        HUDSystem.Instance.Show<MenuPanel>();

    }
    private void RestartAction()
    {
        SaveController.ClearGameData();
        SceneManager.LoadScene(1);
        HUDSystem.Instance.Hide<PausePanel>();
    }
    private void CloseAction()
    {
        HUDSystem.Instance.Hide<PausePanel>();
    }
}
