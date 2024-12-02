using System;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

/// <summary>
/// Represents additional metadata or details associated with a game event.
/// This class stores key-value pairs of details that can be added, retrieved, or cleared.
/// </summary>
[Serializable]
public class EventDetails
{
    /// <summary>
    /// Reference to the parent EventTrigger that this EventDetails belongs to.
    /// </summary>
    [HideInInspector]
    public EventTrigger ParentTrigger;

    /// <summary>
    /// A unique identifier for the event, typically used to track specific events.
    /// </summary>
    public string EventID;

    /// <summary>
    /// Example Sub-Details class for an event
    /// </summary>
    [ShowIf("IsInteractableEvent")]
    public EventInteractableDetails InteractableDetails;

    /// <summary>
    /// A dictionary holding the key-value pairs of event details (metadata).
    /// </summary>
    private readonly Dictionary<string, object> details = new();

    /// <summary>
    /// Adds or updates a detail associated with the event.
    /// </summary>
    /// <param name="key">The key to identify the detail.</param>
    /// <param name="value">The value of the detail.</param>
    public void AddDetail(string key, object value) => details[key] = value;

    /// <summary>
    /// Tries to retrieve a detail by its key.
    /// </summary>
    /// <param name="key">The key associated with the detail to retrieve.</param>
    /// <param name="value">The value of the detail, if found.</param>
    /// <returns>True if the detail was found, otherwise false.</returns>
    public object TryGetDetail(string key, out object value) => details.TryGetValue(key, out value);

    /// <summary>
    /// Clears all the details associated with the event.
    /// </summary>
    public void ClearDetails() => details.Clear();

    /// <summary>
    /// Gets all details as key-value pairs.
    /// </summary>
    /// <returns>An enumerable of key-value pairs representing all event details.</returns>
    public IEnumerable<KeyValuePair<string, object>> GetAllDetails() => details;
}
