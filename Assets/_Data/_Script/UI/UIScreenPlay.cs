using Assets._Data._Script.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreenPlay : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    private void Start()
    {
        bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        ActiveGameobject(sound);
        panelPause.SetActive(false);

    }
    public void PauseGame()
    {
        panelPause.SetActive(true);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetOptionScreenClip());

    }
    public void ContinueGame()
    {
        panelPause.SetActive(false);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());

    }

    public void ReturnHome()
    {
        SaveController.Instance.SaveGame();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());
    }

    public void ReplayGame()
    {
        string sceneCurrent = SceneManager.GetActiveScene().name;
        SaveController.Instance.ClearFile();
        PlayerPrefs.SetInt("isHigh", 0);
        SceneManager.LoadScene(sceneCurrent, LoadSceneMode.Single);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());

    }
    public void SoundSetting()
    {
        bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        sound = !sound;
        ActiveGameobject(sound);
        PlayerPrefs.SetInt("SoundOn", sound == true ? 1 : 0);
        AudioController.Instance.OnOffMusic(sound);
    }

    private void ActiveGameobject(bool sound)
    {
        if (sound)
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
        else
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
    }
    private void OnApplicationQuit()
    {
        if (ShapeController.Instance.endGame == false)
        {
            SaveController.Instance.SaveGame();
        }
    }
}
