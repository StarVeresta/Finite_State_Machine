using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/AI/State")]
public class State : ScriptableObject
{
    public string stateName;
    public bool _isStateActive;
    public string _animationName;
    public Action[] actions;
    public Transition[] transitions;
    public Color gizmoColor = Color.blue;
    public bool isTransitionable = true;


    public void UpdateState(StateMachineEngine controller)
    {
        ExecuteAction(controller);
        CheckForTransition(controller);
    }

    
    private void ExecuteAction(StateMachineEngine controller)
    {
        foreach (var action in actions)
        {
            action.act(controller);
        }
    }

    private void CheckForTransition(StateMachineEngine controller)
    {
        foreach (var transition in transitions)
        {
            bool decision = transition.decision.Decide(controller);
            if (decision)
            {
                controller.TransitionToState(transition.trueState);
            }
            else
            {
                controller.TransitionToState(transition.falseState);
            }
        }
    }

}
