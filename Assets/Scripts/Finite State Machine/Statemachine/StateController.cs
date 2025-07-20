using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Extends the base FSM engine to manage state execution and control activation.
/// Acts as the runtime AI controller for an entity.
/// </summary>
public class StateController : StateMachineEngine
{
    // Controls whether the AI is active and allowed to process logic
    private bool _isActive;

    /// <summary>
    /// Initializes the base FSM and sets up search and activation state.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        IsSearching = false;
        InitialiseAI(true);
    }

    /// <summary>
    /// Enables or disables AI behavior execution.
    /// </summary>
    /// <param name="active">Whether the AI should be active.</param>
    public void InitialiseAI(bool active)
    {
        _isActive = active;
    }

    /// <summary>
    /// Updates the current state each frame if AI is active and valid.
    /// </summary>
    private void Update()
    {
        // Exit early if the AI is disabled
        if (!_isActive) return;

        // Skip update if the entity is dead
        if (_healthHandler != null && !_healthHandler.IsAlive) return;

        // Skip if the agent is not ready
        if (_agent == null || !_agent.enabled) return;

        // Run the logic for the current active state
        currentState.UpdateState(this);
    }

    /// <summary>
    /// Visualizes the state controller in the Scene view for debugging.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}
