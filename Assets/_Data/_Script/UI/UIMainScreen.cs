using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainScreen : MonoBehaviour
{

    private void Start()
    {
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());

    }
    public void PlayGame()
    {
        AudioController.Instance.PlayAudio(AudioAssets.Instance.GetTitleScreenClip());

        SceneManager.LoadScene("ScreenPlay");
    }


}
