using UnityEngine;

public interface IDamagable
{
    bool IsDied { get; }

    void TakeDamage(float damage);
    void Died();
}