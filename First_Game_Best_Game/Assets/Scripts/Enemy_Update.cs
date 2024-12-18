using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Update : MonoBehaviour
{
    // Health
    [SerializeField] private int healthTotal = 0;
    int healthCurrent = 0;

    // Movement Speed
    [SerializeField] private float speedTotal = 0f;
    float speedCurrent = 0f;

    [SerializeField] private float respawnDelay = 1f;
    float delayCurrent = 0f;

    [SerializeField] private int livesTaken = 1;
    
    // Path
    Queue <PathNode> path = new Queue <PathNode>();
    Tuple <Vector2, PathNode> nextPoint = null;

    SpriteRenderer sprite;

    // Events
    [HideInInspector] public UnityEvent spawn = new UnityEvent();
    [HideInInspector] public UnityEvent<int> despawn = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<float> hit = new UnityEvent<float>();

    void Awake()
    {
        if (speedTotal <= 0) Debug.LogError($"Enemy {this.gameObject.name} has ZERO movement");
        else speedCurrent = speedTotal;

        if (healthTotal <= 0) Debug.LogError($"Enemy {this.gameObject.name} has ZERO health");
        else healthCurrent = healthTotal;

        if (livesTaken < 1) 
        {
            Debug.LogWarning($"Enemy {this.gameObject.name} does NOT cause harm");
            livesTaken = 0;
        }
        
        sprite = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)  Debug.LogError($"Enemy {this.gameObject.name} has NO sprite");
        else sprite.enabled = false;

        this.enabled = false;
    }

    public bool CanMove()
    {
        return nextPoint != null;
    }

    public bool Respawning()
    {
        return delayCurrent > 0;
    }

    public bool IsDefeated()
    {
        return healthCurrent <= 0;
    }

    public bool IsActive()
    {
        return !IsDefeated() && !Respawning();
    }

    public void PlaceOnMap(List <PathNode> newPath)
    {
        path.Clear();
        foreach (PathNode node in newPath) path.Enqueue(node);

        // Set position to next point
        SetNextPoint();
        if (nextPoint != null) this.gameObject.transform.position = nextPoint.Item1;

        this.enabled = true;
        sprite.enabled = true;
        delayCurrent = 0;

        spawn.Invoke();
    }

    void SetNextPoint()
    {
        // Reached END of road (despawn)
        if (path.Count == 0)
        {
            nextPoint = null;
            sprite.enabled = false;
            delayCurrent = respawnDelay;
            despawn.Invoke(livesTaken);
            return;
        }

        // Get next node
        PathNode newNode = path.Dequeue();

        Vector2 newPos = newNode.GetLocalPoint();
        nextPoint = Tuple.Create(newPos, newNode);
    }

    void Move()
    {
        if (!CanMove()) return;
       
        Vector2 currentPosition = this.gameObject.transform.position;

        // Calculate step and distance
        float step = speedCurrent * Time.fixedDeltaTime;
        float dist = Vector2.Distance(nextPoint.Item1, currentPosition);

        // Enemy reaches next point
        if (dist < step + Utils.epsilon)
        {
            // Set position to next point
            this.gameObject.transform.position = nextPoint.Item1;
            float difference = step - dist;

            // Set next point
            SetNextPoint();

            // Move by leftover distance
            if (CanMove() && difference > Utils.epsilon) step = difference;
            else return;
        }

        // Calculate new position
        Vector2 newPos = Vector2.MoveTowards(currentPosition, nextPoint.Item1, step);

        // Move to new position
        this.gameObject.transform.position = newPos;
    }

    void TakeDamage(int damage)
    {
        healthCurrent -= damage;

        float healthPercantage = (healthCurrent * 100) / healthTotal;
        hit.Invoke(healthPercantage);
    }

    public void Collide()
    {
        // TODO Colisions
    }

    void FixedUpdate()
    {
        if (Respawning())
        {
            delayCurrent -= Time.fixedDeltaTime;
            if (delayCurrent <= 0) delayCurrent = 0;
        }
        else
        {
            Move();
            Collide();
        }
    }
}
