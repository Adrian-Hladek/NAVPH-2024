using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] private int order = -1;

    [SerializeField] private List <GameObject> enemies = new List <GameObject>();
    [SerializeField] private List <float> spawnTimes = new List <float>();

    Queue <Tuple<GameObject, float>> enemyQueue = new Queue <Tuple<GameObject, float>>();
    List <Enemy_Update> spawnedEnemies = new List <Enemy_Update>();
    float elapsedTime = 0;

    public int WaveOrder
    {
        get{return order;}
    }

    public bool FinishedSpawning()
    {
        return (enemyQueue.Count == 0) && (spawnedEnemies.Count == 0);
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

            if (smallestTime <= elapsedTime) 
            {
                GameObject enemyObject = enemyQueue.Dequeue().Item1;
                enemiesToSpawn.Add(Instantiate(enemyObject));
            }
            else break;
        }

        return enemiesToSpawn;
    }

    public void RemoveDefeatedEnemies()
    {
        List <Enemy_Update> killedEnemies = new List<Enemy_Update>();

        // Remove defeated enemies
        spawnedEnemies.RemoveAll(enemy => 
        {
            if (enemy.IsDefeated()) 
            {
                Debug.Log($"ENEMY DEAD = {enemy.gameObject.name}");
                killedEnemies.Add(enemy);
                return true;
            }
            else return false;
        });

        // Destroy gameObjects of defeated enemies
        foreach (Enemy_Update enemy in killedEnemies) Destroy(enemy.gameObject);
    }

    public void RespawnEnemies(List<PathNode> enemyPath)
    {
        foreach (Enemy_Update enemy in spawnedEnemies)
        {
            if (!enemy.CanMove() && !enemy.Respawning()) enemy.PlaceOnMap(enemyPath);
        }
    }

    public void ActivateEnemy(GameObject enemy, List<PathNode> enemyPath)
    {
        if (!enemy.CompareTag(Utils.enemyTag)) return;

        Enemy_Update enemyComponent = enemy.GetComponentInChildren<Enemy_Update>();
        if (enemyComponent == null) return;

        enemyComponent.PlaceOnMap(enemyPath);
        spawnedEnemies.Add(enemyComponent);
    }
}
