using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



public class Enemy_Update : MonoBehaviour
{
    [Header("Atributes")]
    [SerializeField] private int healthTotal = 0;
    int healthCurrent = 0;

    // Movement Speed
    [SerializeField] private float speedTotal = 0f;
    float speedCurrent = 0f;

    // Respawn delay
    [SerializeField] private float respawnDelay = 1f;
    float delayCurrent = 0f;

    // Lives taken at the nd of road
    [SerializeField] private int livesTaken = 1;

    // Path
    Queue <PathNode> path = new Queue <PathNode>();
    Tuple <Vector2, PathNode> nextPoint = null;

    // UI
    SpriteRenderer sprite;
    Animator animator = null;

    // UI Events
    [HideInInspector] public UnityEvent spawn = new UnityEvent();
    [HideInInspector] public UnityEvent despawn = new UnityEvent();
    [HideInInspector] public UnityEvent<float> hit = new UnityEvent<float>();


    public int GetLivesCost
    {
        get {return livesTaken;}
    }

    void Awake()
    {
        this.enabled = false;

        // Parameters
        if (speedTotal <= 0) Debug.LogError($"Enemy {this.gameObject.name} has ZERO movement");
        else speedCurrent = speedTotal;

        if (healthTotal <= 0) Debug.LogError($"Enemy {this.gameObject.name} has ZERO health");
        else healthCurrent = healthTotal;

        if (livesTaken < 1) 
        {
            Debug.LogWarning($"Enemy {this.gameObject.name} does NOT cause harm");
            livesTaken = 0;
        }
        
        // Sprite
        sprite = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if (sprite == null) Debug.LogError($"Enemy {this.gameObject.name} has NO sprite");
        else sprite.enabled = false;

        // Animation
        animator = this.gameObject.GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError($"Enemy {this.gameObject.name} has NO animation");
    }

    public bool CanMove()
    {
        return nextPoint != null;
    }

    public bool Respawning()
    {
        return delayCurrent > 0;
    }

    public bool StarterRespawning()
    {
        return respawnDelay == delayCurrent;
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

    void UpdateAnimation(Vector2 currentPosition, Vector2 nextPosition)
    {
        if (animator == null) return;

        Direction direction = Utils.DirectionBetweenPoints(currentPosition, nextPosition);

        switch(direction)
        {
            case Direction.Left:
                animator.SetBool("FacingLeft", true);
                animator.SetBool("FacingRight", false);
                animator.SetBool("FacingUp", false);
                animator.SetBool("FacingDown", false);
                break;

            case Direction.Right:
                animator.SetBool("FacingLeft", false);
                animator.SetBool("FacingRight", true);
                animator.SetBool("FacingUp", false);
                animator.SetBool("FacingDown", false);
                break;

            case Direction.Up:
                animator.SetBool("FacingLeft", false);
                animator.SetBool("FacingRight", false);
                animator.SetBool("FacingUp", true);
                animator.SetBool("FacingDown", false);
                break;

            case Direction.Down:
                animator.SetBool("FacingLeft", false);
                animator.SetBool("FacingRight", false);
                animator.SetBool("FacingUp", false);
                animator.SetBool("FacingDown", true);
                break;
        }
    }

    void SetNextPoint()
    {
        // Reached END of road (despawn)
        if (path.Count == 0)
        {
            nextPoint = null;
            sprite.enabled = false;
            delayCurrent = respawnDelay;
            despawn.Invoke();
            return;
        }

        // Get next node
        PathNode newNode = path.Dequeue();

        Vector2 newPos = newNode.GetLocalPoint();
        nextPoint = Tuple.Create(newPos, newNode);

        UpdateAnimation(this.gameObject.transform.position, newPos);
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

    public void TakeDamage(int damage)
    {
        healthCurrent -= damage;

        float healthPercantage = (healthCurrent * 100) / healthTotal;
        hit.Invoke(healthPercantage);
    }

    void FixedUpdate()
    {
        if (Respawning())
        {
            delayCurrent -= Time.fixedDeltaTime;
            if (delayCurrent <= 0) delayCurrent = 0;
        }
        else Move();
    }
}
