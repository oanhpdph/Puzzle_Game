using System.Collections;
using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
#if UNITY_6000
                    instance = FindAnyObjectByType<T>();
#else
                instance = FindObjectOfType<T>();
#endif
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                    //instance.OnMonoSingletonCreated();
                }
            }
            return instance;
        }
    }
}
