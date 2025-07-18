using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Actions/Patrol/Continuous Patrol")]
public class ContinuousPatrol : Action
{
    public override void act(StateMachineEngine controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateMachineEngine controller)
    {
        
    
    }
}
