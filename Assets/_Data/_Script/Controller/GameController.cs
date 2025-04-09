using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance { get; set; }
    public static GameController Instance => instance;

    public float goalScore;
    public float currentScore;
    public bool isHighScore;
    public BlockGenerator blockGenerator;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        isHighScore = PlayerPrefs.GetInt("isHigh") == 1;
        goalScore = PlayerPrefs.GetFloat("goalScore");
        currentScore = PlayerPrefs.GetFloat("currentScore");

    }

}
