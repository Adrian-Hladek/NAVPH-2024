using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] int order = -1;

    [SerializeField] List <GameObject> enemies = new List <GameObject>();
    [SerializeField] List <float> spawnTimes = new List <float>();

    private Queue <Tuple<GameObject, float>> enemyQueue = new Queue <Tuple<GameObject, float>>();
    private List <Enemy_Health> activeEnemies = new List <Enemy_Health>();
    private float elapsedTime = 0;

    public int WaveOrder
    {
        get{return order;}
    }

    public bool IsCompleted()
    {
        return (enemyQueue.Count == 0) && (activeEnemies.Count == 0);
    }

    void Awake()
    {
        if (order < 0 || enemies.Count == 0 || spawnTimes.Count == 0 || spawnTimes.Count != enemies.Count) 
        {
            Debug.LogError($"Wave {this.gameObject.name} NOT properly set");
            enemies.Clear();
            return;
        }

        // Order enemies by spawn time
        List <Tuple<GameObject, float>> enemyList = new List <Tuple<GameObject, float>>();
        for (int i = 0; i < enemies.Count; i++) enemyList.Add(Tuple.Create(enemies[i], spawnTimes[i]));
        enemyList = enemyList.OrderBy(o => o.Item2).ToList();

        // Create enemy queue
        enemyQueue = new Queue <Tuple<GameObject, float>>(enemyList);
    }

    public List <GameObject> UpdateTime(float passedTime)
    {
        List <GameObject> enemiesToSpawn = new List<GameObject>();
        elapsedTime += passedTime;

        while (enemyQueue.Count > 0)
        {
            float smallestTime = enemyQueue.Peek().Item2;

            if (smallestTime <= elapsedTime) enemiesToSpawn.Add(enemyQueue.Dequeue().Item1);
            else break;
        }

        return enemiesToSpawn;
    }

    public void RemoveDefeatedEnemies()
    {
        // Destroy defeated enemies
        foreach (Enemy_Health enemy in activeEnemies)
            if (enemy.EnemyIsDefeated()) Destroy(enemy.gameObject);

        // Remove enemies from active list
        activeEnemies.RemoveAll(enemy => enemy.EnemyIsDefeated());
    }

    public void ActivateEnemy(GameObject enemy, List<PathNode> enemyPath)
    {
        if (!enemy.CompareTag(Utils.enemyTag)) return;

        Enemy_Movement movement = enemy.GetComponentInChildren<Enemy_Movement>();
        Enemy_Health health = enemy.GetComponentInChildren<Enemy_Health>();

        if (movement == null || health == null) return;

        movement.enabled = true;
        health.enabled = true;

        movement.PlaceOnMap(enemyPath);
        activeEnemies.Add(health);
    }
}
