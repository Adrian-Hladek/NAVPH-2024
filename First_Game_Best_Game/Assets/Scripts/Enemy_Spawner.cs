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
    List <Enemy_Update> aliveEnemies = new List <Enemy_Update>();
    float elapsedTime = 0;

    public int WaveOrder
    {
        get{return order;}
    }

    public bool FinishedFighting()
    {
        return (enemyQueue.Count == 0) && (aliveEnemies.Count == 0);
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
        aliveEnemies.RemoveAll(enemy => 
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
        foreach (Enemy_Update enemy in killedEnemies) 
        {
            enemy.spawn.RemoveAllListeners();
            enemy.despawn.RemoveAllListeners();
            enemy.hit.RemoveAllListeners();

            Destroy(enemy.gameObject);
        }
    }

    public int RespawnEnemies(List<PathNode> enemyPath)
    {
        int livesTaken = 0;

        foreach (Enemy_Update enemy in aliveEnemies)
        {
            if (!enemy.CanMove())
            {
                // Respawn enemy
                if (!enemy.Respawning()) enemy.PlaceOnMap(enemyPath);
                // Reduce player lives
                else if (enemy.StarterRespawning()) livesTaken += enemy.GetLivesCost;
            }
        }

        return livesTaken;
    }

    public void ActivateEnemy(GameObject enemy, List<PathNode> enemyPath)
    {
        if (!enemy.CompareTag(Utils.enemyTag)) return;

        Enemy_Update enemyComponent = enemy.GetComponentInChildren<Enemy_Update>();
        if (enemyComponent == null) return;

        enemyComponent.PlaceOnMap(enemyPath);
        aliveEnemies.Add(enemyComponent);
    }
}
