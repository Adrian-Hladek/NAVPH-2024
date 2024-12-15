using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private float speedTotal = 0f;
    private float speedCurrent = 0f;

    [SerializeField] private Tuple <float, float> offsetMinMax = Tuple.Create(0f, 0f);
    private float offset;
    
    Queue <PathNode> path = new Queue <PathNode>();
    Tuple <UnityEngine.Vector2, PathNode> nextPoint = null;

    public bool CanMove()
    {
        return nextPoint != null;
    }

    void Awake()
    {
        if (speedTotal <= 0) Debug.LogError($"Enemy {this.gameObject.name} has no movement");
        else speedCurrent = speedTotal;

        offset = UnityEngine.Random.Range(offsetMinMax.Item1, offsetMinMax.Item2);
        offset = MathF.Round(offset, 4);
    }

    public void Spawn()
    {

    }

    public void PlaceOnMap(List <PathNode> newPath)
    {
        path.Clear();
        foreach (PathNode node in newPath) path.Enqueue(node);

        // Set position to next point
        SetNextPoint();
        if (nextPoint != null) this.gameObject.transform.localPosition = nextPoint.Item1;
    }

    private void SetNextPoint()
    {
        if (path.Count == 0)
        {
            nextPoint = null;
            return;
        }

        // Get next node
        PathNode newNode = path.Dequeue();

        // TODO offset
        UnityEngine.Vector2 newPos = newNode.GetLocalPoint();

        nextPoint = Tuple.Create(newPos, newNode);
    }

    public void MoveEnemy()
    {
        Debug.Log(Time.fixedDeltaTime);

        if (!CanMove()) return;

        UnityEngine.Vector2 currentPosition = this.gameObject.transform.localPosition;

        // Calculate step and distance
        float step = speedCurrent * Time.fixedDeltaTime;
        float dist = UnityEngine.Vector2.Distance(nextPoint.Item1, currentPosition);

        if (dist < step + Utils.epsilon)
        {
            // Set position to next point
            this.gameObject.transform.localPosition = nextPoint.Item1;

            // Get next point
            SetNextPoint();
            if (!CanMove()) return;

            // Move by leftover distance
            float difference = step - dist;
            if (difference > Utils.epsilon) step = difference;
            else return;
        }

        // Calculate new position
        UnityEngine.Vector2 newPos = UnityEngine.Vector2.MoveTowards(currentPosition, nextPoint.Item1, step);

        // Move towards new position
        this.gameObject.transform.localPosition = newPos;
    }
}
