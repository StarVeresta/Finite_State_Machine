using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Core component for driving a state machine using ScriptableObject-based states.
/// Holds references to sensors, animation, and navigation, and manages transitions between states.
/// </summary>
public class StateMachineEngine : MonoBehaviour
{
    [Header("State Configuration")]
    [Tooltip("Initial state to reset to on start.")]
    public State _resetState;

    [Tooltip("Dummy state used to indicate no transition.")]
    public State _remainStats;

    [Tooltip("The current active state.")]
    public State currentState;

    // Internal component references
    [HideInInspector] public AI_VisionSensor _visionSensor;
    [HideInInspector] public AI_audioSensor _audioSensor;
    [HideInInspector] public AIHealth _healthHandler;

    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshAgent _agent;

    // Target tracking
    [HideInInspector] public Transform target;
    [HideInInspector] public Vector3 LastKnownTargetPosition;

    // Search behavior control
    [HideInInspector] public bool IsSearching;
    [HideInInspector] public bool SearchCoroutineRunning;
    [HideInInspector] public Vector3 LastSearchPosition;
    [HideInInspector] public bool IsWanderingSearching = false;

    // Smoothed movement speed calculation
    [HideInInspector] public float _smoothedSpeed = 0f;
    [HideInInspector] public float _speedVelocity = 0f;
    public float smoothTime = 0.15f;

    [Header("Movement Settings")]
    public float WalkRangeRadius = 5f;
    public float SearchDuration = 4f;
    public float SearchPointInterval = 2f;

    /// <summary>
    /// Current position of this agent.
    /// </summary>
    public Vector3 GetCurrentPosition { get { return transform.position; } private set { } }

    /// <summary>
    /// Transitions to a new state if it is not the remain state.
    /// </summary>
    /// <param name="nextState">The next state to transition to.</param>
    public void TransitionToState(State nextState)
    {
        if (nextState != _remainStats)
        {
            currentState = nextState;
        }
    }

    /// <summary>
    /// Initialize internal references on Awake.
    /// </summary>
    private void Awake()
    {
        TryGetComponent<AIHealth>(out _healthHandler);
        TryGetComponent<AI_VisionSensor>(out _visionSensor);
        TryGetComponent<AI_audioSensor>(out _audioSensor);

        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        // Delay agent activation until initial state setup
        _agent.enabled = false;
    }

    /// <summary>
    /// Activates the default reset state and enables the NavMeshAgent.
    /// </summary>
    protected virtual void Start()
    {
        currentState = _resetState;
        _agent.enabled = true;
    }
}
