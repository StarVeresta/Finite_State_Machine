using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Search")]
public class SearchAction : Action
{
    public override void act(StateMachineEngine controller)
    {
        if (controller == null) return;

        Search(controller);
    }

    private void Search(StateMachineEngine controller)
    {

        // Animate based on velocity
        if (controller._animator != null)
        {
            float targetSpeed = Mathf.Clamp(controller._agent.velocity.magnitude, 0, controller._agent.speed);
            controller._smoothedSpeed = Mathf.SmoothDamp(controller._smoothedSpeed, targetSpeed, ref controller._speedVelocity, controller.smoothTime);
            controller._animator.SetFloat("Speed", targetSpeed);
        }

        if (!controller.IsSearching)
        {
            // Move to the last known position
            if (!HasReachedDestination(controller))
            {
                if (controller.LastKnownTargetPosition != Vector3.zero)
                {
                    controller.LastSearchPosition = controller.LastKnownTargetPosition;
                }

                controller._agent.destination = controller.LastSearchPosition;
            }
            else
            {
                controller.IsSearching = true;

                if (!controller.SearchCoroutineRunning)
                {
                    controller.StartCoroutine(SearchArea(controller));
                }
            }
        }
    }

    private bool HasReachedDestination(StateMachineEngine controller)
    {
        if (controller._agent.pathPending || controller._agent == null) return false;

        return controller._agent.remainingDistance <= controller._agent.stoppingDistance &&
               (!controller._agent.hasPath || controller._agent.velocity.sqrMagnitude < 0.01f);
    }

    private IEnumerator SearchArea(StateMachineEngine controller)
    {
        controller.IsSearching = false;
        controller.SearchCoroutineRunning = false;

        // Optional: change to patrol or idle state
        controller.currentState = controller._resetState;
        yield break;
    }

    private Vector3 GetRandomSearchPoint(Vector3 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return center + new Vector3(randomPoint.x, 0, randomPoint.y);
    }

}
