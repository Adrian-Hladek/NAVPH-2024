using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    Queue <Enemy_Spawner> waves;
    Enemy_Spawner currentWave = null;
    Map_Pathing pathing;

    void Awake()
    {
        // Order waves
        List <Enemy_Spawner> waveList = this.gameObject.GetComponentsInChildren<Enemy_Spawner>().ToList();
        waveList = waveList.OrderBy(x => x.WaveOrder).ToList();

        waves = new Queue <Enemy_Spawner>(waveList);
        if (waves.Count == 0) 
        {
            Debug.LogError("Map has NO waves");
            enabled = false;
        }

        pathing = this.gameObject.GetComponentInChildren<Map_Pathing>();
        if (pathing == null) 
        {
            Debug.LogError("Map has NO path");
            enabled = false;
        }

        // TODO remove
        ActivateNextWave();
    }

    public bool LevelCompleted()
    {
        return waves.Count == 0;
    }

    public bool HasActiveWave()
    {
        return currentWave != null;
    }

    public void ActivateNextWave()
    {
        if (HasActiveWave() || LevelCompleted()) return;

        currentWave = waves.Dequeue();
    }

    void FixedUpdate()
    {
        if (HasActiveWave())
        {
            if (!currentWave.FinishedSpawning()) 
            {
                // Spawn new enemies
                List<GameObject> enemiesToSpawn = currentWave.UpdateTime(Time.fixedDeltaTime);
                foreach (GameObject enemy in enemiesToSpawn) currentWave.ActivateEnemy(enemy, pathing.path); 

                // Update enemy list
                currentWave.RemoveDefeatedEnemies();
                currentWave.RespawnEnemies(pathing.path);
            }
            // Deactivate wave
            else currentWave = null;
        }
    }
}
