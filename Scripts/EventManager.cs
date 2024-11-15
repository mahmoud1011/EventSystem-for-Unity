using FYP.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A singleton class responsible for managing events, including subscribing, unsubscribing, and raising events.
/// It uses weak references to handle listeners and subscribers to optimize memory management.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// Unity event that is triggered when an event enters the system. 
    /// This can be used to notify other systems about the event.
    /// </summary>
    public UnityEvent<EventType, string> OnEnterEvent;

    /// <summary>
    /// A dictionary that maps event IDs to a list of weak references to event listeners. 
    /// Weak references help manage memory by allowing the garbage collector to clean up unused listeners.
    /// </summary>
    private Dictionary<string, List<WeakReference<Action<EventBase>>>> eventListeners;

    /// <summary>
    /// A dictionary that maps event IDs to a list of weak references to subscriber GameObjects.
    /// Helps manage memory for objects that only temporarily subscribe to events.
    /// </summary>
    private Dictionary<string, List<WeakReference<GameObject>>> eventSubscriberObjects;

    /// <summary>
    /// Initializes the EventManager by setting up the dictionaries for event listeners and subscribers.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        eventListeners = new Dictionary<string, List<WeakReference<Action<EventBase>>>>();
        eventSubscriberObjects = new Dictionary<string, List<WeakReference<GameObject>>>();
    }

    /// <summary>
    /// Subscribes a listener (action) to a specific event. The listener will be invoked when the event is raised.
    /// </summary>
    /// <param name="eventId">The unique identifier for the event.</param>
    /// <param name="listener">The action to invoke when the event is triggered.</param>
    /// <param name="subscriber">The GameObject subscribing to the event, stored as a weak reference for memory efficiency.</param>
    public void Subscribe(string eventId, Action<EventBase> listener, GameObject subscriber)
    {
        if (!eventListeners.ContainsKey(eventId))
        {
            eventListeners[eventId] = new List<WeakReference<Action<EventBase>>>();
            eventSubscriberObjects[eventId] = new List<WeakReference<GameObject>>();
        }

        eventListeners[eventId].Add(new WeakReference<Action<EventBase>>(listener));
        eventSubscriberObjects[eventId].Add(new WeakReference<GameObject>(subscriber));
    }

    /// <summary>
    /// Unsubscribes a listener from a specific event. The listener will no longer be called when the event is triggered.
    /// </summary>
    /// <param name="eventId">The unique identifier for the event.</param>
    /// <param name="listener">The action that was previously subscribed to the event.</param>
    /// <param name="subscriber">The GameObject that was subscribing to the event.</param>
    public void Unsubscribe(string eventId, Action<EventBase> listener, GameObject subscriber)
    {
        if (eventListeners.ContainsKey(eventId))
        {
            var listenerRefs = eventListeners[eventId];
            var subscriberRefs = eventSubscriberObjects[eventId];

            listenerRefs.RemoveAll(l => l.TryGetTarget(out var target) && target == listener);
            subscriberRefs.RemoveAll(s => s.TryGetTarget(out var target) && target == subscriber);

            if (listenerRefs.Count == 0)
            {
                eventListeners.Remove(eventId);
                eventSubscriberObjects.Remove(eventId);
            }
        }
    }

    /// <summary>
    /// Raises an event and triggers all listeners that are subscribed to it.
    /// If the event's conditions are met, listeners will be invoked.
    /// </summary>
    /// <param name="@event">The event to raise.</param>
    public void RaiseEvent(EventBase @event)
    {
        if (!@event.CanTrigger())
        {
            LogManager.LogError("<color=red><b>Event enter conditions are not met.</b></color>");
            return;
        }

        if (eventListeners.ContainsKey(@event.Details.EventID))
        {
            InvokeListeners(@event.Details.EventID, @event);
        }
    }

    /// <summary>
    /// Invokes all listeners that are subscribed to the specified event ID.
    /// </summary>
    /// <param name="eventId">The unique identifier for the event.</param>
    /// <param name="@event">The event to be passed to the listeners.</param>
    private void InvokeListeners(string eventId, EventBase @event)
    {
        if (eventListeners.TryGetValue(eventId, out var listeners))
        {
            foreach (var weakListener in listeners)
            {
                if (weakListener.TryGetTarget(out var listener))
                {
                    OnEnterEvent?.Invoke(@event.EventType, @event.Details.EventID);
                    listener.Invoke(@event);
                }
            }
        }
    }
}
