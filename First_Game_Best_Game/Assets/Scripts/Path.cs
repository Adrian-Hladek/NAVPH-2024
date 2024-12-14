using UnityEngine;
using System.Collections.Generic;
using System;

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

    public static bool EqualNodes(PathNode left, PathNode right)
    {
        if (left.x == right.x && left.y == right.y) return true;
        return false;
    }

    private static List <PathNode> ReconstructPath(Dictionary <Tuple<float, float>, PathNode> validNodes, Dictionary <Tuple<float, float>, int> nodeDistances, PathNode start, PathNode end)
    {
        Tuple<float, float> startId = start.GetId();
        List <PathNode> shortestPath = new List <PathNode>();
        bool hasStart = false;
    
        PathNode currentNode = validNodes[end.GetId()];
        while(currentNode != null)
        {
            shortestPath.Add(currentNode);

            // Reached start
            if (EqualNodes(currentNode, start)) 
            {
                hasStart = true;
                break;
            }

            int smallestDistance = nodeDistances[currentNode.GetId()];
            PathNode newNode = null;

            foreach (Tuple<float, float> nodeId in currentNode.neighbors)
            {
                int currentDistance = nodeDistances[nodeId];

                if (currentDistance < smallestDistance)
                {
                    newNode = validNodes[nodeId];
                    break;
                }
            }

            currentNode = newNode;
        }

        if (hasStart)
        {
            shortestPath.Reverse();
            return shortestPath;
        }
        
        return null;
    }

    public static List <PathNode> GetShortestPath(PathNode start, PathNode end, Dictionary <Tuple<float, float>, PathNode> validNodes)
    {
        // Unvisited nodes
        Dictionary <Tuple<float, float>, PathNode> unvisited = new Dictionary <Tuple<float, float>, PathNode> (validNodes);

        // Distance of nodes
        Dictionary <Tuple<float, float>, int> nodeDistances = new Dictionary <Tuple<float, float>, int> ();
        foreach (PathNode node in validNodes.Values) nodeDistances.Add(node.GetId(), int.MaxValue);
        nodeDistances[start.GetId()] = 0;

        while (unvisited.Count > 0)
        {
            PathNode bestNode = null;
            int smallestDistance = int.MaxValue;

            // Find best node
            foreach ((Tuple<float, float> id, PathNode node) in unvisited)
            {
                int currentDistance = nodeDistances[id];
                if (currentDistance < smallestDistance) 
                {
                    smallestDistance = currentDistance;
                    bestNode = node;
                }
            }

            // No best node
            if (bestNode == null) break;

            // Update neighbors distances
            foreach (Tuple<float, float> neighborId in bestNode.neighbors)
            {
                int newDistance = smallestDistance + 1;
                int currentDistance = nodeDistances[neighborId];

                if (newDistance < currentDistance) nodeDistances[neighborId] = newDistance;
            }

            unvisited.Remove(bestNode.GetId());
        }

        if (nodeDistances[end.GetId()] != int.MaxValue) 
        {
            Debug.Log($"Shortest distance to end point = {nodeDistances[end.GetId()]}");
            return ReconstructPath(validNodes, nodeDistances, start, end);
        }

        return null;
    }
}

