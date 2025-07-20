using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Handles health, damage response, death behavior, and health bar UI for AI agents.
/// Inherits from the base Health class and adds FSM-specific behavior and animation handling.
/// </summary>
public class AIHealth : Health
{
    [Header("Health References")]
    [SerializeField] private GameObject _healthBarObject;
    [SerializeField] private Image _healthBarFiller;
    [SerializeField] private Color _normalHealthColour = Color.green;
    [SerializeField] private Color _lowHealthColour = Color.red;
    [SerializeField] private float _xpDrop = 25; // Optional XP drop on death
    [SerializeField] private float _fadeDuration = 2f;

    // Reference to where the agent was hit (used for UI rotation)
    public Transform GetHitDirection => _hitPosition;
    private Transform _hitPosition;

    // Reference to the owning FSM controller
    private StateMachineEngine controller;

    /// <summary>
    /// Initializes health values. Called from the base class.
    /// </summary>
    protected override void Initialise()
    {
        _maxHealth = 100f;
        _currentHealth = _maxHealth;

        // AI may not use player save data
        UpdateUI();
    }

    /// <summary>
    /// Unity Start lifecycle hook. Also sets FSM controller reference if available.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        if (TryGetComponent<StateMachineEngine>(out StateMachineEngine stateController))
        {
            controller = stateController;
        }
    }

    /// <summary>
    /// Updates the health UI and rotates the health bar to face the attacker.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (_healthBarObject != null)
        {
            // Show health bar only while taking damage
            _healthBarObject.SetActive(TakingDamage);

            if (GetAttackerPosition != null && TakingDamage)
            {
                // Rotate health bar to face attacker
                Vector3 targetPosition = GetAttackerPosition;
                Vector3 direction = targetPosition - _healthBarObject.transform.position;
                direction.y = 0; // Keep rotation on the Y axis only

                if (direction != Vector3.zero)
                {
                    _healthBarObject.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }

    /// <summary>
    /// Updates the health bar fill amount and color based on current health.
    /// </summary>
    protected override void UpdateUI()
    {
        float health = Mathf.Clamp01(_currentHealth / _maxHealth);

        if (_healthBarFiller != null)
        {
            _healthBarFiller.color = health < 0.3f ? _lowHealthColour : _normalHealthColour;
            _healthBarFiller.fillAmount = health;
        }
    }

    /// <summary>
    /// Handles all logic related to death: disables movement, plays animation, and begins fade sequence.
    /// </summary>
    /// <param name="attacker">The GameObject that caused the death.</param>
    protected override void InitialiseDeath(GameObject attacker)
    {
        base.InitialiseDeath(attacker);

        // Stop and disable NavMeshAgent
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        // Disable Rigidbody and Collider to prevent post-death interactions
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Trigger death animation and start fade coroutine
        if (controller._animator != null)
        {
            controller._animator.SetTrigger("OnDeath");
            StartCoroutine(DeathSequence(attacker));
        }
    }

    /// <summary>
    /// Waits for death animation to play, then fades and destroys the game object.
    /// </summary>
    /// <param name="attacker">The GameObject that killed this AI.</param>
    private IEnumerator DeathSequence(GameObject attacker)
    {
        bool hasEnteredDeathState = false;
        float maxWait = 3f; // Failsafe to prevent infinite loop

        // Wait until death animation state is entered
        while (!hasEnteredDeathState && maxWait > 0f)
        {
            AnimatorStateInfo currentState = controller._animator.GetCurrentAnimatorStateInfo(0);

            // Match your Animator state/tag
            if (currentState.IsName("Death") || currentState.IsTag("Death"))
            {
                hasEnteredDeathState = true;
                yield return new WaitForSeconds(currentState.length);
            }
            else
            {
                maxWait -= Time.deltaTime;
                yield return null;
            }
        }

        // Optionally wait for fade delay before destruction
        yield return new WaitForSeconds(_fadeDuration);

        Destroy(gameObject);
    }
}
