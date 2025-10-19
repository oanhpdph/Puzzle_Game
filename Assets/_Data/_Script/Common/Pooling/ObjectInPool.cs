using System;
using UnityEngine;
using UnityEngine.Pool;

class ObjectInPool : MonoBehaviour, IObjectInPool
{
    public int PoolID { get; private set; }

    public bool inPoolStack;

    private IPoolObject _pool;
    private IOnRecycle[] _recycleCallbacks = Array.Empty<IOnRecycle>();

    public void InitPool(int poolID, IPoolObject poolHandle)
    {
        PoolID = poolID;
        _pool = poolHandle;

        _recycleCallbacks = GetComponents<IOnRecycle>() ?? Array.Empty<IOnRecycle>();
    }

    public void Recycle()
    {
        _pool?.Recycle(PoolID, gameObject);

        foreach (var cb in _recycleCallbacks)
        {
            cb?.OnRecycle();
        }
    }
}