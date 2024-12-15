using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave_Manager : MonoBehaviour
{
    private Queue <Enemy_Spawner> waves;
    private Enemy_Spawner currentWave = null;
    private Map_Pathing pathing;

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
    }

    public void ActivateNextWave()
    {

    }

    public bool HasActiveWave()
    {
        return currentWave != null;
    }

    void FixedUpdate()
    {
        if (HasActiveWave())
        {
            if (!currentWave.IsCompleted()) 
            {
                // Spawn new enemies
                List<GameObject> enemiesToSpawn = currentWave.UpdateTime(Time.fixedDeltaTime);
                foreach (GameObject enemy in enemiesToSpawn) currentWave.ActivateEnemy(enemy, pathing.path); 

                // Update active enemy list
                currentWave.RemoveDefeatedEnemies();
            }
            // Deactivate wave
            else currentWave = null;
        }
    }
}
