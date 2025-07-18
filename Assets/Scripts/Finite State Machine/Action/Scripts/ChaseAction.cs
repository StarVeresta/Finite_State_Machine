using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Chase")]
public class ChaseAction : Action
{
    private bool _isChasing;

    public override void act(StateMachineEngine controller)
    {
        Chase(controller);
    }

    private void Chase(StateMachineEngine controller)
    {
        if (controller._visionSensor == null) return;

        var target = (controller._visionSensor.ObjectFound.Count > 0 && controller._visionSensor.ObjectFound[0] != null)
            ? controller._visionSensor.ObjectFound[0]
            : null;

        // Animate based on velocity
        if (controller._animator != null)
        {
            float targetSpeed = Mathf.Clamp(controller._agent.velocity.magnitude, 0, controller._agent.speed);
            controller._smoothedSpeed = Mathf.SmoothDamp(controller._smoothedSpeed, targetSpeed, ref controller._speedVelocity, controller.smoothTime);
            controller._animator.SetFloat("Speed", targetSpeed);
        }

        if (target != null)
        {
            StartChasing(controller, target.transform);
        }
        else
        {
            if(controller.LastKnownTargetPosition != Vector3.zero)
            {
                MoveToLastKnownPosition(controller);
            }
        }
    }

    private void StartChasing(StateMachineEngine controller, Transform target)
    {
        controller._agent.destination = target.position;
        controller.LastKnownTargetPosition = target.position;
        controller.target = target;
        controller._agent.isStopped = false;
    }

    private void MoveToLastKnownPosition(StateMachineEngine controller)
    {
        controller._agent.destination = controller.LastKnownTargetPosition;
        controller._agent.isStopped = false;
    }

}
