using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Listener<T>
{
    public Action<T> action;
    public MonoBehaviour objectListener;
    public int priority;


    public Listener(Action<T> action, MonoBehaviour objectListener, int priority = 0)
    {
        this.action = action;
        this.objectListener = objectListener;
        this.priority = priority;
    }
}


public class Events<T>
{
    public List<Listener<T>> listeners;

    public Events()
    {
        if (listeners == null)
            listeners = new List<Listener<T>>();
    }

    public void Trigger(T param1)
    {
        var initialListenersCount = listeners.Count;
        for (var i = 0; i < listeners.Count; i++)
        {
            var listener = listeners[i];
            if (listener == null || listener.objectListener == null || listener.action == null)
                Remove(listener.action);
            else
                listener.action?.Invoke(param1);
            if (initialListenersCount != listeners.Count)
            {
                i--;
                initialListenersCount = listeners.Count;
                OrderListeners();
            }
        }
    }

    public void Insert(Action<T> action, MonoBehaviour objectListener, int priority = 0)
    {
        if (listeners == null)
            listeners = new List<Listener<T>>();

        var listener = new Listener<T>(action, objectListener, priority);
        listeners.Add(listener);
        OrderListeners();
    }

    public void Remove(Action<T> action)
    {
        var listener = listeners.FirstOrDefault(x => x.action == action);
        listeners?.Remove(listener);
        OrderListeners();
    }
    public void RemoveAllInstances(Action<T> action)
    {
        listeners?.RemoveAll(x => x.action == action);
        OrderListeners();
    }

    private void OrderListeners() => listeners = listeners.OrderByDescending(x => x.priority).ToList();
    public void Clear() => listeners.Clear();
}
