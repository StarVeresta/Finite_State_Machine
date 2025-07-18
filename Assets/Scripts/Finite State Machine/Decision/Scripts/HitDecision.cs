using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Decision/Health")]
public class HitDecision : Decision
{
    public override bool Decide(StateMachineEngine controller)
    {
        return TakeDamageFunction(controller);
    }

    private bool TakeDamageFunction(StateMachineEngine controller)
    {
        if (controller._healthHandler == null) return false;

        if (controller._healthHandler.TakingDamage)
        {
            Transform targetPosition = controller._healthHandler.GetHitDirection;

            if (targetPosition != null)
            {
                controller.target = targetPosition.transform;
                return true;
            }
        }
        return false;
    }
}
