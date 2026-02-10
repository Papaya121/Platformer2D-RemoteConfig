using System;
using UnityEngine;

public class PlayerHealthController : HealthController
{

    [Header("Collision")]
    [SerializeField] private LayerMask enemyLayers;
    public event Action OnPlayerDied;

    private bool _isDead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDieFromLayer(collision.gameObject.layer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDieFromLayer(other.gameObject.layer);
    }

    private void TryDieFromLayer(int layer)
    {
        if (((1 << layer) & enemyLayers) != 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (_isDead)
            return;

        _isDead = true;
        Debug.Log("Player has died.");
        OnPlayerDied?.Invoke();
    }
}
