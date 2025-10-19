using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class LoadingPanel : Panel
{
    private readonly float timeToLoad = 2f;
    private float timeCount = 0f;

    public void StartLoading()
    {
        StartCoroutine(Loading());
    }
    public void StartLoadingPlay()
    {
        StartCoroutine(LoadingPlay());
    }
    private IEnumerator Loading()
    {
        while (timeCount < timeToLoad)
        {
            yield return null;
            timeCount += 0.2f;
        }
        timeCount = 0;
        HUDSystem.Instance.Show<MenuPanel>();
        HUDSystem.Instance.Hide<LoadingPanel>();
    }
    private IEnumerator LoadingPlay()
    {
        while (timeCount < timeToLoad)
        {
            yield return null;
            timeCount += 0.2f;
        }
        timeCount = 0;
        HUDSystem.Instance.Hide<LoadingPanel>();
    }
}
