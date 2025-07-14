using UnityEngine;


public class BtnPauseGame : MonoBehaviour
{

    public void PauseGame()
    {

        GameController.Instance.CurrentState = StateGame.pause;
    }
}
