using System.Collections;
using UnityEngine;


public static class PlayerData
{
    public static bool Music
    {
        get { return PlayerPrefs.GetInt(FlagsData.MUSIC, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(FlagsData.MUSIC, value == true ? 1 : 0);
        }
    }

}
