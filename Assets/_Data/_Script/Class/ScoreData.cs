using System;

[Serializable]
public class ScoreData
{
    public float currentScore;
    public float goalScore;
    public bool isHighScore;

    public ScoreData ResetData()
    {
        currentScore = 0; isHighScore = false;
        return this;
    }
}
