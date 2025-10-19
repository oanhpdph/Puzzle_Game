using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPanel : Panel
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button musicBtn;
    [SerializeField] private Image On;
    [SerializeField] private Image Off;
    private bool isMusic = true;
    private void Start()
    {
        playBtn.AddListener<object>(_ => PlayAction(), Listener.OnClick);
        musicBtn.AddListener<object>(_ => MusicAction(), Listener.OnClick);
        isMusic = !PlayerData.Music;
        MusicAction();
    }
    private void OnEnable()
    {
        isMusic = !PlayerData.Music;
        MusicAction();
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetOptionScreenClip());
    }
    private void PlayAction()
    {
        GameController.Instance.ReLoadData();
        SceneManager.LoadScene("ScreenPlay");
        HUDSystem.Instance.Hide<MenuPanel>();
        HUDSystem.Instance.Show<LoadingPanel>().StartLoadingPlay();

    }
    private void MusicAction()
    {
        this.isMusic = !isMusic;
        PlayerData.Music = isMusic;
        AudioController.Instance.OnOffMusic(isMusic);
        SetUp(isMusic);
    }
    private void SetUp(bool isMusicOn)
    {
        On.gameObject.SetActive(isMusicOn);
        Off.gameObject.SetActive(!isMusicOn);
    }
}
