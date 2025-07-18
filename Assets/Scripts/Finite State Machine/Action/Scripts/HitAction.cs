using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Hit")]
public class HitAction : Action
{
    [SerializeField] private State SearchState;

    public override void act(StateMachineEngine controller)
    {
        MoveToHitPosition(controller);
    }

    private void MoveToHitPosition(StateMachineEngine controller)
    {
        if(controller == null) return;

        if(controller._healthHandler.GetHitDirection != null)
        {
            controller._agent.SetDestination(controller.target.position);
        }

        if (HasReachedDestination(controller))
        {
            controller.currentState = SearchState;
        }

    }

    private bool HasReachedDestination(StateMachineEngine controller)
    {
        if (controller._agent.pathPending) return false;

        // Check if the AI has reached the target location
        return !controller._agent.pathPending && controller._agent.remainingDistance <= controller._agent.stoppingDistance &&
               (!controller._agent.hasPath || controller._agent.velocity.sqrMagnitude == 0f);
    }

}

