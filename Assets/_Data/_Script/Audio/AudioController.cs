using System.Threading.Tasks;
using UnityEngine;

public class AudioController : PersistentSingleton<AudioController>
{
    [SerializeField] private AudioSource music;

    [SerializeField] private AudioSource SFX;

    protected override void Awake()
    {
        base.Awake();
        OnOffMusic(PlayerData.Music);
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
