using UnityEngine;

/// <summary>
/// Base class for all decisions used in the FSM transition system.
/// Inherit from this to define conditional logic that determines if a state transition should occur.
/// </summary>
public abstract class Decision : ScriptableObject
{
    /// <summary>
    /// Evaluates the decision condition.
    /// </summary>
    /// <param name="controller">The current state machine executing this decision.</param>
    /// <returns>True if the decision condition is met and transition should occur; otherwise false.</returns>
    public abstract bool Decide(StateMachineEngine controller);
}
