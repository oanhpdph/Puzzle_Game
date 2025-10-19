using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public interface IAddListener
{
    void AddListeners<T>(Selectable uiElement, Action<T> action, Listener listener);

}
