using UnityEngine;

public class AudioAssets : MonoBehaviour
{
    private static AudioAssets instance { get; set; }
    public static AudioAssets Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip optionScreenClip;
    [SerializeField] private AudioClip titleScreenClip;
    [SerializeField] private AudioClip scoreScreenClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip collidingClip;


    public AudioClip GetGameOverClip() { return gameOverClip; }
    public AudioClip GetOptionScreenClip() { return optionScreenClip; }
    public AudioClip GetTitleScreenClip() { return titleScreenClip; }
    public AudioClip GetScoreScreenClip() { return scoreScreenClip; }
    public AudioClip GetWinClip() { return winClip; }
    public AudioClip GetCollidingClip() { return collidingClip; }


}
