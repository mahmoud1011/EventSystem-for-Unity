using System;

/// <summary>
/// Represents a condition that must be met in order for an event to be triggered. 
/// It uses a delegate function to evaluate the condition.
/// </summary>
public class EventCondition : ICondition
{
    /// <summary>
    /// The function that defines the condition logic. It is expected to return a boolean value
    /// indicating whether the condition is met (true) or not (false).
    /// </summary>
    private readonly Func<bool> conditionFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventCondition"/> class with the given condition function.
    /// </summary>
    /// <param name="conditionFunc">A delegate (Func<bool>) that evaluates the condition.</param>
    public EventCondition(Func<bool> conditionFunc)
        => this.conditionFunc = conditionFunc;

    /// <summary>
    /// Evaluates the condition by invoking the provided delegate function.
    /// </summary>
    /// <returns>True if the condition is met, otherwise false.</returns>
    public bool Evaluate()
        => conditionFunc(); 
}
