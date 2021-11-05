using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, UnityEvent<string>> eventDictionaryString;
    private Dictionary<string, UnityEvent<GameManager.Behavior>> eventDictionaryBehavior;

    private static EventManager eventManager;

    private static EventManager instance
    {
        get
        {
            if (eventManager) return eventManager;

            eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

            if (!eventManager)
            {
                Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
            }
            else
            {
                eventManager.Init();
            }

            return eventManager;
        }
    }

    private void Init()
    {
        eventDictionary ??= new Dictionary<string, UnityEvent>();
        eventDictionaryString ??= new Dictionary<string, UnityEvent<string>>();
        eventDictionaryBehavior ??= new Dictionary<string, UnityEvent<GameManager.Behavior>>();
    }

    // No parameters
    public static void StartListening(string eventName, UnityAction listener)
    {
        if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;

        if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        if (instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    // String parameter
    public static void StartListening(string eventName, UnityAction<string> listener)
    {
        if (instance.eventDictionaryString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string>();
            thisEvent.AddListener(listener);
            instance.eventDictionaryString.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<string> listener)
    {
        if (eventManager == null) return;

        if (instance.eventDictionaryString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, string message)
    {
        if (instance.eventDictionaryString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            thisEvent.Invoke(message);
        }
    }

    // Behavior parameter
    public static void StartListening(string eventName, UnityAction<GameManager.Behavior> listener)
    {
        if (instance.eventDictionaryBehavior.TryGetValue(eventName, out UnityEvent<GameManager.Behavior> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<GameManager.Behavior>();
            thisEvent.AddListener(listener);
            instance.eventDictionaryBehavior.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<GameManager.Behavior> listener)
    {
        if (eventManager == null) return;

        if (instance.eventDictionaryBehavior.TryGetValue(eventName, out UnityEvent<GameManager.Behavior> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, GameManager.Behavior message)
    {
        if (instance.eventDictionaryBehavior.TryGetValue(eventName, out UnityEvent<GameManager.Behavior> thisEvent))
        {
            thisEvent.Invoke(message);
        }
    }

}