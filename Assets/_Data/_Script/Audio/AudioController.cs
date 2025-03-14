using UnityEngine;

public class AudioController : MonoBehaviour
{
    private static AudioController instance { get; set; }
    public static AudioController Instance => instance;
    [SerializeField] private AudioSource music;

    [SerializeField] private AudioSource SFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;//1 is on 0 is off
        OnOffMusic(sound);
    }
    public void PlayAudio(AudioClip audio)
    {
        music.clip = audio;
        music.Play();
    }
    public void OnOffMusic(bool isOn)
    {
        if (isOn)
        {
            music.volume = 1;
            SFX.volume = 1;
        }
        else
        {
            music.volume = 0;
            SFX.volume = 0;
        }
    }
    public void PlayOneShot(AudioClip audio)
    {
        music.PlayOneShot(audio);
    }

}
