using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int health_total = 0;
    private int health_current;

    void Awake()
    {
        if (health_total <= 0) Debug.LogError($"Enemy {this.gameObject.name} has no health");
        else health_current = health_total;

        enabled = false;
    }

    public bool EnemyIsDefeated()
    {
        return health_current <= 0;
    }

    private void TakeDamage(int damage)
    {
        health_current -= damage;

    }

    void FixedUpdate()
    {
        // TODO Colisions
    }
}
