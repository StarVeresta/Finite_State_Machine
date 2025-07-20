using UnityEngine;

/// <summary>
/// Defines a single state in the AI finite state machine.
/// Encapsulates logic for executing actions and evaluating transitions based on decisions.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/AI/State")]
public class State : ScriptableObject
{
    [Header("State Settings")]
    public string stateName;

    [Tooltip("Marks this state as active for debugging or runtime control.")]
    public bool _isStateActive;

    [Tooltip("Optional animation trigger or name associated with this state.")]
    public string _animationName;

    [Tooltip("Actions that are executed every frame while this state is active.")]
    public Action[] actions;

    [Tooltip("Transitions evaluated each frame to determine if the state should change.")]
    public Transition[] transitions;

    [Tooltip("Gizmo color used to visualize this state in the editor.")]
    public Color gizmoColor = Color.blue;

    [Tooltip("Determines if this state is allowed to transition to others.")]
    public bool isTransitionable = true;

    /// <summary>
    /// Called every frame by the FSM. Runs actions and checks transitions.
    /// </summary>
    /// <param name="controller">The FSM controller currently running this state.</param>
    public void UpdateState(StateMachineEngine controller)
    {
        ExecuteAction(controller);
        CheckForTransition(controller);
    }

    /// <summary>
    /// Executes all actions assigned to this state.
    /// </summary>
    private void ExecuteAction(StateMachineEngine controller)
    {
        foreach (var action in actions)
        {
            action.act(controller);
        }
    }

    /// <summary>
    /// Evaluates transitions and attempts to switch states based on decisions.
    /// </summary>
    private void CheckForTransition(StateMachineEngine controller)
    {
        foreach (var transition in transitions)
        {
            bool decision = transition.decision.Decide(controller);

            if (decision)
            {
                controller.TransitionToState(transition.trueState);
            }
            else
            {
                controller.TransitionToState(transition.falseState);
            }
        }
    }
}
