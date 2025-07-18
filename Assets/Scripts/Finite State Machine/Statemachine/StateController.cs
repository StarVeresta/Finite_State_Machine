using UnityEngine;
using UnityEngine.AI;

public class StateController : StateMachineEngine
{
    private bool _isActive;

    protected override void Start()
    {
        base.Start();

        IsSearching = false;
        InitialiseAI(true);
    }

    public void InitialiseAI(bool active)
    {
        _isActive = active;
    }

    private void Update()
    {
        if (!_isActive) return;

        if (_healthHandler != null && !_healthHandler.IsAlive) return;

        if (_agent == null || !_agent.enabled) return;

        currentState.UpdateState(this);
    }

    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }

}
