using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Wander")]
public class WanderAction : Action
{
    public override void act(StateMachineEngine controller)
    {
        Wander(controller);
    }

    private void Wander(StateMachineEngine controller)
    {
        controller.IsSearching = controller.IsWanderingSearching;

        if (controller._agent == null)
        {
            Debug.LogWarning("NavMeshAgent is null. WanderAction aborted.");
            return;
        }

        // Animate based on velocity
        if (controller._animator != null)
        {
            float targetSpeed = Mathf.Clamp(controller._agent.velocity.magnitude, 0, controller._agent.speed);
            controller._smoothedSpeed = Mathf.SmoothDamp(controller._smoothedSpeed, targetSpeed, ref controller._speedVelocity, controller.smoothTime);
            controller._animator.SetFloat("Speed", targetSpeed);
        }

        if (!controller.IsWanderingSearching)
        {
            if (HasReachedDestination(controller))
            {
                Vector3 randomWanderPoint = GetRandomSearchPoint(controller.GetCurrentPosition, controller.WalkRangeRadius, controller.WalkRangeRadius);

                controller._agent.isStopped = false;
                controller._agent.SetDestination(randomWanderPoint);
            }
            else
            {
                // Still moving to destination do nothing special here
            }
        }


        // Check if we just reached destination and not already searching, then start searching
        if (!controller.IsWanderingSearching && HasReachedDestination(controller))
        {
            controller.IsWanderingSearching = true;
            controller.StartCoroutine(SearchArea(controller));
        }
    }



    private bool HasReachedDestination(StateMachineEngine controller)
    {
        var agent = controller._agent;

        bool reached = !agent.pathPending &&
                       agent.remainingDistance <= agent.stoppingDistance &&
                       (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        return reached;
    }

    private IEnumerator SearchArea(StateMachineEngine controller)
    {
        float searchEndTime = Time.time + controller.SearchDuration;
        while(Time.time < searchEndTime)
        {
            Vector3 randomSearchPoint = GetRandomSearchPoint(controller.GetCurrentPosition, controller.WalkRangeRadius, controller.WalkRangeRadius);
            controller._agent.SetDestination(randomSearchPoint);
            yield return new WaitUntil(() => HasReachedDestination(controller) || Time.time >= searchEndTime);
            yield return new WaitForSeconds(controller.SearchPointInterval);
        }

        controller.IsWanderingSearching = false;
        controller.currentState = controller._resetState;
    }

    private Vector3 GetRandomSearchPoint(Vector3 center, float searchRadius, float allowedRadius)
    {
        Vector3 randomPoint;
        int attempts = 10;

        do
        {
            Vector2 random2D = Random.insideUnitCircle * searchRadius;
            randomPoint = center + new Vector3(random2D.x, 0, random2D.y);
            attempts--;
        }
        while (Vector3.Distance(center, randomPoint) > allowedRadius && attempts > 0);

        return randomPoint;
    }


}
