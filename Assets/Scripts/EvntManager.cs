using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EvntManager : MonoBehaviour
{
    private Dictionary<string, UnityEventBase> eventDictionary;

    private static EvntManager eventManager;

    public static EvntManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EvntManager)) as EvntManager;

                if (!eventManager)
                {
                    Debug.LogError("There is no active event manager on your scene!!!");
                }
                else
                {
                    eventManager.Initialize();
                }
            }

            return eventManager;
        }
    }

    void Initialize()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEventBase>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            thisEvent = existingEvent as UnityEvent;
        }
        else
        {
            thisEvent = new UnityEvent();
            Instance.eventDictionary.Add(eventName, thisEvent);
        }

        thisEvent.AddListener(listener);
    }

    public static void StartListening<T>(string eventName, UnityAction<T> listener)
    {
        UnityEvent<T> thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            thisEvent = existingEvent as UnityEvent<T>;
        }
        else
        {
            thisEvent = new UnityEvent<T>();
            Instance.eventDictionary.Add(eventName, thisEvent);
        }

        thisEvent.AddListener(listener);
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;

        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            UnityEvent thisEvent = existingEvent as UnityEvent;
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening<T>(string eventName, UnityAction<T> listener)
    {
        if (eventManager == null) return;

        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            UnityEvent<T> thisEvent = existingEvent as UnityEvent<T>;
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            UnityEvent thisEvent = existingEvent as UnityEvent;
            thisEvent.Invoke();
        }
    }

    public static void TriggerEvent<T>(string eventName, T value)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEventBase existingEvent))
        {
            UnityEvent<T> thisEvent = existingEvent as UnityEvent<T>;
            thisEvent.Invoke(value);
        }
    }
}
