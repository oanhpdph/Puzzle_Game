using System;

[Serializable]
public class ScoreData
{
    public float currentScore;
    public float goalScore;
    public bool isHighScore;
    public int combo;
    public ScoreData ResetData()
    {
        currentScore = 0; isHighScore = false; combo = 0;
        return this;
    }
}
