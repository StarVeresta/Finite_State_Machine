using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/AI/Actions/Patrol")]
public class PatrolAction : Action
{
    private Coroutine _idleCoroutine;
    private float _idleRate = 0; // Ensures idleChance is only calculated once per waypoint

    public override void act(StateMachineEngine controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateMachineEngine controller)
    {
        // move the AI depending on the Statecontroller
    }

    private IEnumerator HandleIdle(StateMachineEngine controller)
    {
        yield return null;
    }

    private void MoveToNextWaypoint(StateMachineEngine controller)
    {

    }

}
