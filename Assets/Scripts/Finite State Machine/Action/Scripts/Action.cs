using UnityEngine;

public abstract class Action : ScriptableObject
{
    public abstract void act(StateMachineEngine controller);


}
