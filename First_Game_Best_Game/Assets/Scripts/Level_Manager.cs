using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum LevelState
{
    Editing = 0, 
    Fighting = 1,
    Finished = 2, 
    GameOver = 3, 
}

public class Level_Manager : MonoBehaviour
{
    Queue <Enemy_Spawner> waves;
    Enemy_Spawner currentWave;
    Map_Pathing pathing;
    LevelState currentState;
    Action_Inventory inventory;

    [SerializeField] private int playerHealth = 0;

    [HideInInspector] public UnityEvent<LevelState> changedLevelState = new UnityEvent<LevelState>();
    [HideInInspector] public UnityEvent<int> changedLivesCount = new UnityEvent<int>();

    void Awake()
    {
        // Order waves
        List <Enemy_Spawner> waveList = this.gameObject.GetComponentsInChildren<Enemy_Spawner>().ToList();
        waveList = waveList.OrderBy(x => x.WaveOrder).ToList();

        if (waveList.Count == 0) 
        {
            Debug.LogError("Level has NO waves");
            enabled = false;
            return;
        }
        else waves = new Queue <Enemy_Spawner> (waveList);

        pathing = FindObjectOfType<Map_Pathing>();
        if (pathing == null) 
        {
            Debug.LogError("Level has NO path");
            enabled = false;
            return;
        }

        inventory = FindObjectOfType<Action_Inventory>();
        if (inventory == null) 
        {
            Debug.LogError("Level has NO inventory");
            enabled = false;
            return;
        }

        if (playerHealth <= 0) 
        {
            Debug.LogError("Player has NO health");
            playerHealth = 0;
        }

        currentWave = null;
        currentState = LevelState.Editing;
        changedLivesCount.Invoke(playerHealth);
    }

    // TODO - remove
    void Start()
    {
        ActivateNextWave();
    }

    public bool LevelIsPlaying()
    {
        return (currentState == LevelState.Editing) || (currentState == LevelState.Fighting);
    }

    public bool LevelIsPaused()
    {
        return Time.timeScale == 0;
    }

    // FixedUpdate/Update is not called while timeScale == 0
    public void SetLevelSpeed(float speed = 0)
    {
        if (currentState != LevelState.Editing && currentState != LevelState.Fighting) return;
        
        Time.timeScale = speed;
    }

    public void ActivateNextWave()
    {
        if (currentState != LevelState.Editing || waves.Count == 0) return;

        currentWave = waves.Dequeue();
        currentState = LevelState.Fighting;

        inventory.UpdateDisabledActions(currentState);
        changedLevelState.Invoke(currentState);
    }

    void ChangePlayerHealth(int livesDifference)
    {
        playerHealth += livesDifference;
        changedLivesCount.Invoke(playerHealth);

        if (playerHealth <= 0) 
        {
            currentState = LevelState.GameOver;

            inventory.UpdateDisabledActions(currentState);
            changedLevelState.Invoke(currentState);
        }
    }

    void FixedUpdate()
    {
        // Pause game
        if (!LevelIsPlaying())
        {
            Time.timeScale = 0;
            return;
        }

        // Update wave
        if (currentState == LevelState.Fighting && currentWave != null)
        {
            if (!currentWave.FinishedFighting()) 
            {
                // Spawn new enemies
                List<GameObject> enemiesToSpawn = currentWave.UpdateTime(Time.fixedDeltaTime);
                foreach (GameObject enemy in enemiesToSpawn) currentWave.ActivateEnemy(enemy, pathing.path); 

                // Update enemy list
                currentWave.RemoveDefeatedEnemies();
                int livesLost = 0 - currentWave.RespawnEnemies(pathing.path);

                if (livesLost < 0) ChangePlayerHealth(livesLost);
            }
            // Deactivate wave
            else 
            {
                currentWave = null;
                currentState = LevelState.Editing;

                inventory.UpdateDisabledActions(currentState);
                changedLevelState.Invoke(currentState);
            }
        }

        // Level won
        else if (waves.Count == 0) 
        {
            currentState = LevelState.Finished;

            inventory.UpdateDisabledActions(currentState);
            changedLevelState.Invoke(currentState);
        }
    }
}
