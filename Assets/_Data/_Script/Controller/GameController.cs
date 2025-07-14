using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance { get; set; }
    public static GameController Instance => instance;

    public delegate void OnDropShape();
    public event OnDropShape onDropShape;
    public event Action<StateGame> OnChangeState = delegate { };
    public StateGame currentState;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void DropShapeAction()
    {
        onDropShape?.Invoke();
    }

    public StateGame CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;
            OnChangeState(currentState);
        }
    }
}
