using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AI/Decision/Hear")]
public class HearDecision : Decision
{
    public override bool Decide(StateMachineEngine controller)
    {
        return Hear(controller);    
    }

    private bool Hear(StateMachineEngine controller)
    {
        if(controller._audioSensor == null) return false;

        if(controller._audioSensor.SoundDetected)
        {
            Transform soundPosition = controller._audioSensor.ObjectLocation();

            if(soundPosition != null)
            {
                controller.LastKnownTargetPosition = soundPosition.transform.position;
                return true;
            }
        }
        return false;
    }
    
}
