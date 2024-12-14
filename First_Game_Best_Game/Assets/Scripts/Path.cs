using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.Experimental.GraphView;


public class PathNode
{
    private float x;
    private float y;
    public HashSet<Tuple<float, float>> neighbors = new HashSet<Tuple<float, float>>();

    private Cell_Update cell;

    public PathNode(Cell_Update cell, Vector3 center_point)
    {
        Vector3 vec = cell.gameObject.transform.InverseTransformPoint(center_point);

        this.x = MathF.Round(vec.x, 2);
        this.y = MathF.Round(vec.y, 2);
        this.cell = cell;
    }

    public Cell_Update cellUpdate
    {
        get {return this.cell;}
    }

    public Tuple<float, float> GetId()
    {
        return Tuple.Create(this.x, this.y);
    }

    public void addNeighbor(PathNode newNeighbor)
    {
        this.neighbors.Add(newNeighbor.GetId());
    }

    public static bool CompareNodes(PathNode left, PathNode right)
{
    if (left.x == right.x && left.y == right.y) return true;
    return false;
}

private void reconstructPath()
{

}

public static void GetShortestPath(PathNode start, PathNode end, List <PathNode> validNodes)
{
    // Unvisited nodes
    Dictionary <Tuple<float, float>, PathNode> unvisited = new Dictionary <Tuple<float, float>, PathNode> ();
    foreach (PathNode node in validNodes) unvisited.Add(node.GetId(), node);

    // Distance of nodes
    Dictionary <Tuple<float, float>, int> nodeDistances = new Dictionary <Tuple<float, float>, int> ();
    foreach (PathNode node in validNodes) nodeDistances.Add(node.GetId(), int.MaxValue);
    nodeDistances[start.GetId()] = 0;

    while (unvisited.Count > 0)
    {
        PathNode best_node = null;
        int smallest_distance = int.MaxValue;

        // Find best node
        // TODO - optimize search
        foreach ((Tuple<float, float> id, PathNode node) in unvisited)
        {
            int current_distance = nodeDistances[id];
            if (current_distance < smallest_distance) 
            {
                smallest_distance = current_distance;
                best_node = node;
            }
        }

        // No best node
        if (best_node == null) break;

        // Update neighbors distances
        foreach (Tuple<float, float> neighborId in best_node.neighbors)
        {
            int new_distance = smallest_distance + 1;
            int current_distance = nodeDistances[neighborId];

            if (new_distance < current_distance) nodeDistances[neighborId] = new_distance;
        }

        unvisited.Remove(best_node.GetId());
    }

    
    if (nodeDistances[end.GetId()] != int.MaxValue) 
    {
        Debug.Log($"Shortest distance to end point = {nodeDistances[end.GetId()]}");
    }
}
}

