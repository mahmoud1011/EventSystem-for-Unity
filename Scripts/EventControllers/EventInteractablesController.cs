using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ReadOnlyAttribute = MyBox.ReadOnlyAttribute;

[Serializable]
public class EventInteractableDetails
{
    [TextArea]
    public string Description;

    public bool RequiresPlayerInteraction;
}

/// <summary>
/// Manages interactable objects and handles interactions triggered by events. It subscribes to interaction events and manages the state of interactables in the game.
/// </summary>
public class EventInteractablesController : Singleton<EventInteractablesController>
{
    /// <summary>
    /// A structure to hold information about each interactable object.
    /// </summary>
    [Serializable]
    public struct InteractableInfo
    {
        /// <summary>
        /// The unique ID associated with the interactable object.
        /// </summary>
        public string interactableId;

        /// <summary>
        /// A read-only flag indicating whether the interactable object has already been interacted with.
        /// </summary>
        [HideInInspector, ReadOnly]
        public bool isInteracted;
    }

    /// <summary>
    /// A list of interactable objects in the game, each with a unique ID and interaction status.
    /// </summary>
    public List<InteractableInfo> interactables;

    /// <summary>
    /// Contains the details of the currently selected interactable object (e.g., description, interaction requirements).
    /// </summary>
    [SerializeField, ReadOnly]
    private EventInteractableDetails currentInteractableDetails;

    private Dictionary<string, InteractableInfo> interactableMap;
    private Dictionary<string, bool> interactableStatus;

    #region ==Init==
    /// <summary>
    /// Initializes the interactable objects by creating mappings for interactable data and their statuses.
    /// Also subscribes to interaction events for each interactable.
    /// </summary>
    private void Start()
    {
        InitInteractableMap();
        InitInteractableStatus();

        // Subscribe to each interactable's interaction event
        foreach (InteractableInfo info in interactables)
        {
            EventManager.Instance.Subscribe(info.interactableId, OnInteractionStart, this.gameObject);
        }
    }

    /// <summary>
    /// Unsubscribes from all interaction events when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            foreach (InteractableInfo info in interactables)
            {
                EventManager.Instance.Unsubscribe(info.interactableId, OnInteractionStart, this.gameObject);
            }
        }
    }

    /// <summary>
    /// Initializes a dictionary mapping each interactable's ID to its corresponding `InteractableInfo` data.
    /// </summary>
    private void InitInteractableMap()
    {
        interactableMap = new Dictionary<string, InteractableInfo>();
        foreach (var interactable in interactables)
        {
            interactableMap[interactable.interactableId] = interactable;
        }
    }

    /// <summary>
    /// Initializes the interaction status of each interactable. Loads saved states from `GlobalConfigs`.
    /// </summary>
    private void InitInteractableStatus()
    {
        interactableStatus = new Dictionary<string, bool>();

        // Load saved states from GlobalConfigs or initialize based on interactable data
        foreach (var interactable in interactables)
        {
            interactableStatus[interactable.interactableId] = interactable.isInteracted;
        }
    }
    #endregion ==Init==

    #region --Callback--
    /// <summary>
    /// This callback is triggered when an interaction event starts. It checks if the interaction is valid and, if so, processes it.
    /// </summary>
    /// <param name="@event">The event that triggered the interaction.</param>
    private void OnInteractionStart(EventBase @event)
    {
        if ((bool)@event.Details.TryGetDetail("id", out object interactableID))
        {
            // Update the details of the current interactable object
            currentInteractableDetails = @event.Details.ParentTrigger.eventDetails.InteractableDetails;

            // Only process interaction if it's the first time (if triggerOnce is set)
            if (@event.Details.ParentTrigger.triggerOnce && !interactableStatus[interactableID.ToString()])
            {
                InteractWithObject(interactableID.ToString());
            }
        }
        else
        {
            LogManager.LogError("Interaction event triggered without valid ID.");
        }
    }

    /// <summary>
    /// Handles timed events by waiting for a delay and then invoking the associated Unity event.
    /// </summary>
    /// <param name="delay">The amount of time to wait before invoking the event.</param>
    /// <param name="timedEvent">The Unity event to be invoked after the delay.</param>
    /// <returns>An IEnumerator for yielding in a coroutine.</returns>
    IEnumerator HandleTimedEvent(float delay, UnityEvent timedEvent)
    {
        yield return new WaitForSeconds(delay);

        timedEvent?.Invoke();
    }

    /// <summary>
    /// Processes the interaction with an interactable object. Updates its interaction status and state.
    /// </summary>
    /// <param name="interactableID">The unique ID of the interactable object to interact with.</param>
    private void InteractWithObject(string interactableID)
    {
        if (interactableMap.TryGetValue(interactableID, out InteractableInfo interactableData))
        {
            // Mark the object as interacted
            interactableData.isInteracted = true;

            // Update the map and status to reflect the interaction
            interactableMap[interactableID] = interactableData;
            interactableStatus[interactableID] = true; // Mark as interacted.
        }
        else
        {
            Debug.LogError($"Interactable ID not found: {interactableID}");
        }
    }
    #endregion --Callback--
}
