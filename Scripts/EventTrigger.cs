using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class manages event triggers based on collider interactions (enter, exit, stay).
/// It listens for trigger events (e.g., player entering an area) and raises game events based on conditions.
/// The events can be triggered once or multiple times depending on the configuration.
/// </summary>
public class EventTrigger : MonoBehaviour
{
    /// <summary>
    /// The type of event this trigger is associated with (e.g., cutscene, dialogue, etc.).
    /// </summary>
    public EventType eventType;

    /// <summary>
    /// Contains additional details related to the event being triggered.
    /// </summary>
    public EventDetails eventDetails = new();

    /// <summary>
    /// Specifies if the event should only be triggered once.
    /// </summary>
    public bool triggerOnce;

    /// <summary>
    /// The collider component that triggers the event when other colliders interact with it.
    /// </summary>
    public Collider enterCollider;

    /// <summary>
    /// A string representing the condition that needs to be met for the event to be triggered.
    /// </summary>
    public string triggerCondition;

    /// <summary>
    /// A list of tags that specify which objects (based on their tags) can trigger the event.
    /// </summary>
    public List<string> triggerTags;

    /// <summary>
    /// Unity event to trigger actions when an object enters the trigger zone.
    /// </summary>
    public UnityEvent OnEnter;

    /// <summary>
    /// Unity event to trigger actions when an object exits the trigger zone.
    /// </summary>
    public UnityEvent OnExit;

    private EventBase currentEvent;
    private bool enterTriggered = false;
    private bool exitTriggered = false;

    #region ==Validate==
    /// <summary>
    /// Called when the object is reset in the editor. Ensures the trigger collider is properly set up if not already assigned.
    /// </summary>
    private void Reset()
    {
        if (!TryGetComponent(out enterCollider))
        {
            enterCollider = gameObject.AddComponent<BoxCollider>();
            enterCollider.isTrigger = true;
        }
    }

    /// <summary>
    /// Validate the parent trigger to corresponding EventType assigned to this TriggerEvent GameObject
    /// </summary>
    private void OnValidate() => eventDetails.ParentTrigger = this;
    #endregion ==Validate==

    #region ==Collision==
    /// <summary>
    /// Called when another collider enters the trigger collider. If the collider has a matching tag, it triggers the event.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (triggerTags.Contains(other.tag))
        {
            SetEnterEvent();
        }
    }

    /// <summary>
    /// Called when another collider stays inside the trigger collider. This can be used for continuous checks, though not currently used.
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (triggerTags.Contains(other.tag))
        {
            // Can be expanded for continuous events
        }
    }

    /// <summary>
    /// Called when another collider exits the trigger collider. If the collider has a matching tag, it triggers the exit event.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (triggerTags.Contains(other.tag))
        {
            SetExitEvent();
        }
    }
    #endregion ==Collision==

    #region ==Fire Events==
    /// <summary>
    /// Triggers the enter event when the conditions are met. If `triggerOnce` is true, it will only trigger once.
    /// </summary>
    public void SetEnterEvent()
    {
        if (triggerOnce && enterTriggered) return;

        PopulateEventDetails();

        currentEvent = new EventBase(eventType, this.gameObject, eventDetails);
        currentEvent.AddCondition(new EventCondition(() => ConditionMet()));

        if (currentEvent.CanTrigger())
        {
            OnEnter?.Invoke();
            EventManager.Instance.RaiseEvent(currentEvent);

            if (triggerOnce)
                enterTriggered = true;
        }
    }

    /// <summary>
    /// Triggers the exit event when the conditions are met. If `triggerOnce` is true, it will only trigger once.
    /// </summary>
    public void SetExitEvent()
    {
        if (triggerOnce && exitTriggered) return;

        if (currentEvent.CanTrigger())
        {
            OnExit?.Invoke();

            if (triggerOnce)
                exitTriggered = true;
        }
    }
    #endregion ==Fire Events==

    #region ==Event Details==
    /// <summary>
    /// Populates the event details based on the event type. Different types of events (cutscene, battle, etc.) can store specific details.
    /// </summary>
    private void PopulateEventDetails()
    {
        eventDetails.ClearDetails();
        eventDetails.AddDetail("id", eventDetails.EventID);

        switch (eventType)
        {
            case EventType.Cutscene:
                // Additional cutscene details can be added here
                break;
            case EventType.Dialogue:
                // Additional dialogue details can be added here
                break;
            case EventType.Battle:
                // Additional battle details can be added here
                break;
            case EventType.CustomEvent:
                // Additional custom event details can be added here
                break;
            case EventType.Interactable:
                eventDetails.AddDetail("details", eventDetails.InteractableDetails);
                break;
            default:
                break;
        }
    }
    #endregion ==Event Details==

    #region ==Condition Checks==
    /// <summary>
    /// Checks if the condition for triggering the event is met. This method is based on the triggerCondition string and evaluates it accordingly.
    /// </summary>
    bool ConditionMet()
    {
        return triggerCondition switch
        {
            "HPFull" => HPFull(),
            "HasKeyItem" => HasKeyItem(),
            "CanFinishLevel" => CanFinishLevel(),
            _ => true, // Default condition
        };
    }

    /// <summary>
    /// Checks if the player's HP is full. Placeholder method for condition checking.
    /// </summary>
    bool HPFull() => PlayerManager.PlayerHP.HPFull();

    /// <summary>
    /// Checks if the player has a key item in their inventory. Placeholder method for condition checking.
    /// </summary>
    bool HasKeyItem() => /*InventoryManager.Instance.HasItem("KeyItem")*/ true;

    /// <summary>
    /// Checks if all enemy groups are defeated, allowing the level to be finished. Placeholder method for condition checking.
    /// </summary>
    bool CanFinishLevel() => EnemyManager.Instance.AreGroupsEmpty(groupIDs);
    #endregion ==Condition Checks==
}
