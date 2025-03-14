using UnityEngine;

namespace Assets._Data._Script.Audio
{
    public class SoundSetting : MonoBehaviour
    {
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;
        // Use this for initialization
        void Start()
        {
            bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;
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

        public void Setting()
        {
            bool sound = PlayerPrefs.GetInt("SoundOn", 1) == 1;
            sound = !sound;
            if (sound)
            {
                PlayerPrefs.SetInt("SoundOn", 1);
                soundOn.SetActive(true);
                soundOff.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("SoundOn", 0);
                soundOn.SetActive(false);
                soundOff.SetActive(true);
            }
            AudioController.Instance.OnOffMusic(sound);
        }
    }
}