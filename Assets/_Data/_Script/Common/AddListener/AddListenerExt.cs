using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public static class AddListenerExt
{
    private readonly static IAddListener addListener = new AddListener();

    public static void AddListener<T>(this Selectable uiElement, Action<T> action, Listener listener)
        => addListener.AddListeners(uiElement, action, listener);
}
