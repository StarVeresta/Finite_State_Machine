using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Decision/Attack")]
public class AttackDecision : Decision
{
    public override bool Decide(StateMachineEngine controller)
    {
        return Attack(controller);
    }

    private bool Attack(StateMachineEngine controller)
    {
        if (controller == null) return false;

        return false;
    }

}
