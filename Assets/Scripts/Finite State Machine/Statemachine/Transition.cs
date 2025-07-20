using UnityEngine;

/// <summary>
/// Represents a transition between states in the FSM, 
/// based on the evaluation of a Decision.
/// </summary>
[System.Serializable]
public class Transition
{
    /// <summary>
    /// The decision to evaluate to determine which state to transition to.
    /// </summary>
    public Decision decision;

    /// <summary>
    /// The state to transition to if the decision evaluates to true.
    /// </summary>
    public State trueState;

    /// <summary>
    /// The state to transition to if the decision evaluates to false.
    /// </summary>
    public State falseState;
}
