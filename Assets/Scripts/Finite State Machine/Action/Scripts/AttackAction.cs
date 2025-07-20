using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Attack")]
public class AttackAction : Action
{
    public override void act(StateMachineEngine controller)
    {
        Attack(controller);
    }

    private void Attack(StateMachineEngine controller)
    {
        if (controller == null) return;

        if (controller.target == null)
        {
            // if there is no target reset state -- target killed or escaped
            controller.currentState = controller._remainStats;
        }

        AttackValidation(controller);
    }

    private void AttackValidation(StateMachineEngine controller)
    {
        
    }

}
