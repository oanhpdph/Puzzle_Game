using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum Listener
{
    OnClick,
    OnChangeValue,

}
public class AddListener : IAddListener
{
    public void AddListeners<T>(Selectable uiElement, Action<T> action, Listener listener)
    {
        switch (uiElement)
        {
            case Button btn:
                EventListener(btn, action, listener);
                break;
            case Slider sld:
                EventListener(sld, action, listener);
                break;
        }
    }
    private void EventListener<T>(Button btn, Action<T> action, Listener listener)
    {
        if (listener == Listener.OnClick)
        {
            btn.onClick.AddListener(() => action?.Invoke(default));
        }
    }

    private void EventListener<T>(Slider sld, Action<T> action, Listener listener)
    {
        if (listener == Listener.OnChangeValue)
        {
            sld.onValueChanged.AddListener(value => action?.Invoke((T)(object)value));
        }
    }

    private void EventListener<T>(Toggle tgl, Action<T> action, Listener listener)
    {
        if (listener == Listener.OnChangeValue)
        {
            tgl.onValueChanged.AddListener(state => action?.Invoke((T)(object)state));
        }
    }
}
