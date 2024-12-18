using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map_Pathing : MonoBehaviour
{
    [SerializeField] private GameObject pathStart = null;
    [SerializeField] private GameObject pathEnd = null;

    List<PathNode> validPath = new List<PathNode>();
    bool validInit = true;

    public List<PathNode> path
    {
        get {return validPath;}
    }
    
    public bool hasValidPath()
    {
        return validPath.Count > 0;
    }

    void Awake()
    {
        if (pathStart == null || !pathStart.CompareTag(Utils.pathTag))
        {
            Debug.LogError("Path START point is INVALID");
            validInit = false;
        }

        if (pathEnd == null || !pathEnd.CompareTag(Utils.pathTag))
        {
            Debug.LogError("Path END point is INVALID");
            validInit = false;
        }
    }

    Cell_Update getEdgeCell(GameObject edge)
    {
        Collider2D pathCollider = edge.GetComponentInChildren<Collider2D>();
        if (pathCollider == null) return null;

        Collider2D[] collisions = new Collider2D[10];
        int overlapCount = Physics2D.OverlapCollider(pathCollider, new ContactFilter2D().NoFilter(), collisions);

        for (int i = 0; i < overlapCount; i++)
        {
            if (collisions[i].gameObject.CompareTag(Utils.cellTag))
            {
                Cell_Update cell = collisions[i].gameObject.GetComponent<Cell_Update>();
                if (cell != null) return cell;
            }
        }

        return null;
    }

    void UpdatePath()
    {
        if (!validInit) return;

        validPath.Clear();

        // Get first and last cell
        Cell_Update start = getEdgeCell(pathStart);
        Cell_Update end = getEdgeCell(pathEnd);
        if (start == null || end == null )
        {
            Debug.LogError("Could not FIND valid path START");
            return;
        }

        // Path edge does NOT connect
        if (!start.HasPath || !end.HasPath) return;

        Vector3 currentPosition = this.gameObject.transform.position;

        // Initiate nodes
        Dictionary <Tuple<float, float>, PathNode> validNodes = new Dictionary <Tuple<float, float>, PathNode>();
        Queue <PathNode> newNodes = new Queue<PathNode> ();
        
        PathNode startNode = new PathNode(start.gameObject, currentPosition, start);
        PathNode endNode = new PathNode(end.gameObject, currentPosition, end);

        // Get valid paths
        newNodes.Enqueue(startNode);
        while (newNodes.Count != 0)
        {
            PathNode currentNode = newNodes.Dequeue();
            List<Cell_Update> neighborCells = currentNode.cellUpdate.findNeighborCells();

            foreach (Cell_Update cell in neighborCells)
            {
                PathNode neighborNode = new PathNode(cell.gameObject, currentPosition, cell);

                // Add neighbor
                currentNode.addNeighbor(neighborNode);

                // Ignore visited and queued nodes
                if (newNodes.Any(x => PathNode.EqualNodes(x, neighborNode)) 
                    || validNodes.ContainsKey(neighborNode.GetId())
                ) continue;
                
                newNodes.Enqueue(neighborNode);
            }

            validNodes.Add(currentNode.GetId(), currentNode);
        }

        // No valid connection
        if (!validNodes.ContainsKey(endNode.GetId())) return;

        // Set new path
        List<PathNode> newPath = PathNode.GetShortestPath(startNode, endNode, validNodes);
        if (newPath != null) 
        {
            PathNode spawn = new PathNode(pathStart, currentPosition);
            PathNode despawn = new PathNode(pathEnd, currentPosition);

            newPath.Insert(0, spawn);
            newPath.Add(despawn);

            validPath = newPath;
        }
        else Debug.LogError("Pathfinding failed");
    }

    void Start()
    {
        UpdatePath();

        // Find Action_Inventory
        Action_Inventory inventory = FindObjectOfType<Action_Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Cound NOT find Action_Inventory, path will NOT be updated");
            return;
        }

        // Add listener to performed action
        inventory.actionPerformed.AddListener(UpdatePath);
    }

}
