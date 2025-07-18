using UnityEngine;
using UnityEngine.AI;

public class StateMachineEngine : MonoBehaviour
{
    public State _resetState;
    public State _remainStats;
    public State currentState;

    [HideInInspector] public AI_VisionSensor _visionSensor;
    [HideInInspector] public AI_audioSensor _audioSensor;
    [HideInInspector] public AIHealth _healthHandler;

    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _agent;
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 LastKnownTargetPosition;
    [HideInInspector] public bool IsSearching;
    [HideInInspector] public bool SearchCoroutineRunning;
    [HideInInspector] public Vector3 LastSearchPosition;
    [HideInInspector] public bool IsWanderingSearching = false;

    [HideInInspector] public float _smoothedSpeed = 0f;
    [HideInInspector] public float _speedVelocity = 0f;
    public float smoothTime = 0.15f;

    public float WalkRangeRadius = 5f;
    public float SearchDuration = 4f;
    public float SearchPointInterval = 2f;
    public Vector3 GetCurrentPosition { get { return transform.position; } private set { } }

    public void TransitionToState(State nextState)
    {
        if (nextState != _remainStats)
        {
            currentState = nextState;
        }
    }

    private void Awake()
    {
        // _combatHandler = GetComponent<AI_CombatHandler>();
        TryGetComponent<AIHealth>(out _healthHandler);
        TryGetComponent<AI_VisionSensor>(out _visionSensor);
        TryGetComponent<AI_audioSensor>(out _audioSensor);

        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
    }

    protected virtual void Start()
    {
        currentState = _resetState;
        _agent.enabled = true;
    }

}
