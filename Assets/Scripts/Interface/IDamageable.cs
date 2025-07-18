using UnityEngine;

public interface IDamageable
{
    public void Damage(float amount, GameObject gameObject);

    public GameObject GetObject();
}
