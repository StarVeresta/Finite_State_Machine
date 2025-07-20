using UnityEngine;

/// <summary>
/// Base class for all actions used in the FSM system.
/// Inherit from this to define reusable behavior logic executed by states.
/// </summary>
public abstract class Action : ScriptableObject
{
    /// <summary>
    /// Called every frame while the state this action belongs to is active.
    /// Override this method to define the action's behavior.
    /// </summary>
    /// <param name="controller">The current state machine executing this action.</param>
    public abstract void act(StateMachineEngine controller);
}
