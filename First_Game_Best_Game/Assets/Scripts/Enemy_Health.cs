using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Enemy_Health : MonoBehaviour
{

    [SerializeField] private int health_total = 0;
    private int health_current;

    void Awake()
    {
        if (health_total <= 0) Debug.LogError($"Enemy {this.gameObject.name} has no health");
        else health_current = health_total;
    }

    private bool EnemyDefeated()
    {
        return this.health_current <= 0;
    }

    public void Collide()
    {
        // TODO
    }

    public void TakeDamage(int damage)
    {
        health_current -= damage;
    }
}
