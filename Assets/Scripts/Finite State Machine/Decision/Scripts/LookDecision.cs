using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/AI/Decision/Look")]
public class LookDecision : Decision
{
    public override bool Decide(StateMachineEngine controller)
    {
        return Look(controller);
    }

    private bool Look(StateMachineEngine controller)
    {
        if (controller._visionSensor == null) return false;

        if(controller._visionSensor.ObjectFound.Count > 0)
        {

            GameObject targetObj = controller._visionSensor.ObjectFound[0];

            if (targetObj != null && targetObj.TryGetComponent<ITargettable>(out ITargettable target))
            {
                controller.target = target.SetTarget();
                return true;
            }
        }
        
        return false;
    }
    

}
