using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class LoaderContext : PersistentSingleton<LoaderContext>
{
    private IPoolObject _pool;

    private void Start()
    {
        InitContext();
    }
    public void InitContext()
    {
        _pool = GetComponent<PoolObject>();
        _pool.Init();
    }
}
