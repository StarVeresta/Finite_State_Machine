using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHealth = 100f;
    [SerializeField] protected float _currentHealth;
    [SerializeField] protected float _hitDuration = 1.5f;

    private float _hitTick;
    private bool _initialised = false;
    public bool IsAlive { get; private set; } = true;
    public bool TakingDamage { get; private set; }

    public float GetHealth => _currentHealth;
    public float GetMaxHealth => _maxHealth;
    public Vector3 GetAttackerPosition {  get; private set; }

    protected virtual void Awake()
    {
        // Subscribed in derived classes
    }

    protected virtual void Start()
    {
        Initialise();
    }

    public void Damage(float amount, GameObject attacker)
    {
        if (amount <= 0 || attacker == null || !IsAlive) return;

        _hitTick = _hitDuration;
        TakingDamage = true;
        _currentHealth -= amount;
        GetAttackerPosition = attacker.transform.position;  

        if (_currentHealth <= 0f)
        {
            _currentHealth = 0;
            IsAlive = false;
            InitialiseDeath(attacker);
        }

        Debug.Log($"{gameObject.name} is taking damage.");
        UpdateUI();
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || !IsAlive) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        UpdateUI();
    }

    public void LevelUpHealth(float growth)
    {
        _maxHealth += _maxHealth * growth;
        _currentHealth = _maxHealth;
        UpdateUI();
    }

    public GameObject GetObject() => this.gameObject;

    protected virtual void Update()
    {
        if (TakingDamage)
        {
            _hitTick -= Time.deltaTime;
            if (_hitTick <= 0)
            {
                TakingDamage = false;
            }
        }
    }

    protected virtual void Initialise()
    {
        if (_initialised) return;
        _initialised = true;
        // This will be overridden in child classes
    }

    protected virtual void UpdateUI() { }

    protected virtual void InitialiseDeath(GameObject attacker) { }
}
