﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystem : MonoBehaviour
{
    public delegate void EventListener(EventInfo e);
    Dictionary<Type, List<EventHandler>> eventListeners;


    public static EventSystem Current;
    internal static UnityEngine.EventSystems.EventSystem InternalCurrent;

    public void Awake()
    {
        Current = this;
    }


    public void RegisterListener<T>(Action<T> listener) where T : EventInfo
    {
        Type eventType = typeof(T);

        var target = listener.Target;
        var method = listener.Method;

        if (eventListeners == null)
        {
            eventListeners = new Dictionary<Type, List<EventHandler>>();
        }

        if (eventListeners.ContainsKey(eventType) == false || eventListeners[eventType] == null)
        {
            eventListeners[eventType] = new List<EventHandler>();
        }
        eventListeners[eventType].Add(new EventHandler { Target = target, Method = method });
    }
    public void UnregisterListener<T>(System.Action<T> listener) where T : EventInfo
    {
        var eventType = typeof(T);
        if (eventListeners != null)
            if (eventListeners.ContainsKey(eventType) == true || eventListeners[eventType] != null)
                eventListeners[eventType].RemoveAll(l => l.Target == listener.Target && l.Method == listener.Method);
    }

    public void FireEvent(EventInfo eventInfo)
    {
        Type trueEventInfoClass = eventInfo.GetType();
        if (eventListeners == null || eventListeners[trueEventInfoClass] == null)
        {
            return;
        }
        foreach (EventHandler el in eventListeners[trueEventInfoClass])
        {
            el.Method.Invoke(el.Target, new[] { eventInfo });
        }
    }
    public class EventHandler
    {
        public object Target { get; set; }
        public System.Reflection.MethodInfo Method { get; set; }
    }
}