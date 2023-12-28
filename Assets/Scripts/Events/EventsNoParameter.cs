using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Listener
{
    public Action action;
    public MonoBehaviour objectListener;
    public int priority;


    public Listener(Action action, MonoBehaviour objectListener, int priority = 0)
    {
        this.action = action;
        this.objectListener = objectListener;
        this.priority = priority;
    }
}


public class Events
{
    public List<Listener> listeners;

    public Events()
    {
        if (listeners == null)
            listeners = new List<Listener>();
    }

    public void Trigger()
    {
        var initialListenersCount = listeners.Count;
        for (var i = 0; i < listeners.Count; i++)
        {
            var listener = listeners[i];
            if (listener == null || listener.objectListener == null || listener.action == null)
                Remove(listener.action);
            else
                listener.action?.Invoke();
            if (initialListenersCount != listeners.Count)
            {
                i--;
                initialListenersCount = listeners.Count;
                OrderListeners();
            }
        }
    }

    public void Insert(Action action, MonoBehaviour objectListener, int priority = 0)
    {
        var listener = new Listener(action, objectListener, priority);
        listeners.Add(listener);
        OrderListeners();
    }

    public void Remove(Action action)
    {
        var listener = listeners.FirstOrDefault(x => x.action == action);
        listeners?.Remove(listener);
        OrderListeners();
    }
    public void RemoveAllInstances(Action action)
    {
        listeners?.RemoveAll(x => x.action == action);
        OrderListeners();
    }

    public void Clear() => listeners.Clear();

    private void OrderListeners() => listeners = listeners.OrderByDescending(x => x.priority).ToList();
}
