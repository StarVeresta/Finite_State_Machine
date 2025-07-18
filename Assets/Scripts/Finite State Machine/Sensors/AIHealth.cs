using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIHealth : Health
{
    [Header("Health References")]
    [SerializeField] private GameObject _healthBarObject;
    [SerializeField] private Image _healthBarFiller;
    [SerializeField] private Color _normalHealthColour = Color.green;
    [SerializeField] private Color _lowHealthColour = Color.red;
    [SerializeField] private float _xpDrop = 25;
    [SerializeField] private float _fadeDuration = 2f;

    public Transform GetHitDirection => _hitPosition;
    private Transform _hitPosition;
    private StateMachineEngine controller;

    protected override void Initialise()
    {
        _maxHealth = 100f;
        _currentHealth = _maxHealth;

        // Optional: Skip loading from DataManager for AI.
        UpdateUI();
    }

    protected override void Start()
    {
        base.Start();

        if (TryGetComponent<StateMachineEngine>(out StateMachineEngine stateController))
        {
            controller = stateController;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_healthBarObject != null)
        {
            _healthBarObject.SetActive(TakingDamage);

            if (GetAttackerPosition != null && TakingDamage)
            {
                // Face the attacker, keeping Y rotation only
                Vector3 targetPosition = GetAttackerPosition;
                Vector3 direction = targetPosition - _healthBarObject.transform.position;
                direction.y = 0; // Lock Y-axis to avoid tilting

                if (direction != Vector3.zero)
                {
                    _healthBarObject.transform.rotation = Quaternion.LookRotation(direction);
                }
            }

        }
    }

    protected override void UpdateUI()
    {
        float health = Mathf.Clamp01(_currentHealth / _maxHealth);

        if (_healthBarFiller != null)
        {
            _healthBarFiller.color = health < 0.3f ? _lowHealthColour : _normalHealthColour;
            _healthBarFiller.fillAmount = health;
        }
    }

    protected override void InitialiseDeath(GameObject attacker)
    {
        base.InitialiseDeath(attacker);

        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false;
        }

        // Disable physics and collisions
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

       

        if (controller._animator != null)
        {
            controller._animator.SetTrigger("OnDeath");
            StartCoroutine(DeathSequence(attacker));
        }
    }

    private IEnumerator DeathSequence(GameObject attacker)
    {
        
        // Wait until the "Death" animation state is entered
        bool hasEnteredDeathState = false;
        float maxWait = 3f; // Failsafe to prevent infinite loop

        while (!hasEnteredDeathState && maxWait > 0f)
        {
            AnimatorStateInfo currentState = controller._animator.GetCurrentAnimatorStateInfo(0);

            // Adjust the name/tag to match your Animator
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

        // Step 3: Wait for fade duration
        yield return new WaitForSeconds(_fadeDuration);

        
        Destroy(gameObject);
    }
}
