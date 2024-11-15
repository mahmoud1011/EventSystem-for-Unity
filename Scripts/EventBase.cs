using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the different types of events that can occur in the game.
/// </summary>
public enum EventType
{
    Cutscene,

    Dialogue,

    Battle,

    Interactable,

    CustomEvent
}

/// <summary>
/// A base class representing a game event. It contains details about the event, its type, 
/// its sender (the game object triggering the event), and the conditions required for the event to be triggered.
/// </summary>
public class EventBase
{
    /// <summary>
    /// Gets the type of the event (e.g., Cutscene, Battle, etc.).
    /// </summary>
    public EventType EventType { get; private set; }

    /// <summary>
    /// Gets the GameObject that triggered this event.
    /// </summary>
    public GameObject Sender { get; private set; }

    /// <summary>
    /// Gets the details associated with this event (e.g., additional metadata).
    /// </summary>
    public EventDetails Details { get; private set; }

    /// <summary>
    /// Gets a list of conditions that must be met in order for the event to be triggered.
    /// </summary>
    public List<ICondition> Conditions { get; private set; } = new List<ICondition>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBase"/> class.
    /// </summary>
    /// <param name="eventType">The type of the event (e.g., Cutscene, Battle).</param>
    /// <param name="sender">The GameObject that is triggering the event.</param>
    /// <param name="details">Optional additional details associated with the event.</param>
    public EventBase(EventType eventType, GameObject sender, EventDetails details = null)
    {
        EventType = eventType;
        Sender = sender;
        Details = details ?? new EventDetails(); // If no details are provided, create a new EventDetails instance
    }

    /// <summary>
    /// Adds a condition that must be met for the event to be triggered.
    /// </summary>
    /// <param name="condition">The condition to be added.</param>
    public void AddCondition(ICondition condition) => Conditions.Add(condition);

    /// <summary>
    /// Checks if all conditions for triggering the event are met.
    /// </summary>
    /// <returns>True if all conditions are met, otherwise false.</returns>
    public bool CanTrigger()
    {
        foreach (var condition in Conditions)
        {
            if (!condition.Evaluate()) 
            {
                return false;
            }
        }
        return true;
    }
}

