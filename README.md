<a id="readme-top"></a>

# EventSystem-for-Unity
EventSystem for Unity designed to efficiently manage game events, facilitating dynamic interactions like cutscenes, battles, and custom sequences.

1. **EventManager**:
   - A Singleton class that manages event subscribers and dispatches events.
   - **Core Methods**:
      - Subscribe(string eventId, Action<EventBase> listener, GameObject subscriber): Register an event listener.
      - Unsubscribe(string eventId, Action<EventBase> listener, GameObject subscriber): Remove an event listener.
      - RaiseEvent(EventBase @event): Trigger an event and notify all registered listeners.
        
2. **EventBase**: 
   - This class serves as the foundational structure for game events. Each event associates with a sender, is defined by an EventType, and includes conditions for triggering.
   - **Key Methods**:
      - AddCondition(ICondition condition): Add conditions for event activation.
      - CanTrigger(): Evaluate conditions to determine if the event should be triggered.
        
3. **EventDetails**: 
   - This class holds information about specific game events, which can be categorized as cutscene, battle, interactable, or custom events.
   - **Properties & Methods**:
      - EventID: Unique identifier for each event.
      - AddDetail(string key, object value): Add or update event metadata.
      - TryGetDetail(string key, out object value): Retrieve specific event details.
      - ClearDetails(): Remove all stored details.

4. **EventCondition**:
   - Implements conditions for events. You define a condition using a delegate (`Func<bool>`) that evaluates the logic required to trigger an event.

5. **EventTrigger**:
   - Manages the firing of events based on interactions with trigger areas (e.g., colliders).
   - **Features**:
      - enterCollider: Collider component that functions as an event trigger.
      - triggerCondition: Specifies conditions for event activation, like player states or item possession.
      - OnEnter and OnExit UnityEvents: Trigger actions when entering or exiting the event area.

## Example Usage
Here’s how you might use these components in your game:

   ```cs
    // Set up an event trigger for a cutscene
    var eventTrigger = new EventTrigger();
    eventTrigger.eventType = EventType.Cutscene;
    eventTrigger.triggerTags = new List<string> { "Player" };
    eventTrigger.triggerOnce = true;
    eventTrigger.OnEnter.AddListener(() => Debug.Log("Cutscene started"));
    
    // Create an event based on conditions
    EventBase myEvent = new EventBase(EventType.Battle, this.gameObject);
    myEvent.AddCondition(new EventCondition(() => PlayerHasSword()));
    if (myEvent.CanTrigger())
    {
        EventManager.Instance.RaiseEvent(myEvent);
    }
    
    // Define the condition logic
    bool PlayerHasSword()
    {
        return InventoryManager.Instance.HasItem("Sword");
    }
   ```

