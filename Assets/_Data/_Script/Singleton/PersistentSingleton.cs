using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<T>();
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
