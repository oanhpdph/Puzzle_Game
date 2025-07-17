using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreenPlay : MonoBehaviour
{
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;
    [SerializeField] private CellGenerator cellManager;

    public BlockGenerator blockGenerator;
    private ISaveLoad saveLoad = new SaveLoad();
    private void Start()
    {
        bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        ActiveGameobject(sound);
    }

    public void ContinueGame()
    {
        GameController.Instance.CurrentState = StateGame.play;

    }

    public void ReturnHome()
    {
        Save();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());
    }

    public void ReplayGame()
    {

        saveLoad.Save(Flags.SCORE_DATA_FILE, GameController.Instance.scoreData.ResetData());
        saveLoad.ClearFile(Flags.CELL_DATA_FILE);
        saveLoad.Save(Flags.BLOCK_DATA_FILE, new SaveDataShape());

        string sceneCurrent = SceneManager.GetActiveScene().name;
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
        Save();
    }
    private void Save()
    {
        saveLoad.Save(Flags.BLOCK_DATA_FILE, blockGenerator.GetBlockData());
        saveLoad.Save(Flags.CELL_DATA_FILE, cellManager.GetAllCell());
        saveLoad.Save(Flags.SCORE_DATA_FILE, GameController.Instance.scoreData);
    }
}

public enum StateGame
{
    play,
    pause,
    end
}
