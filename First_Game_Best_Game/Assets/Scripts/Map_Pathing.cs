using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map_pathing : MonoBehaviour
{
    [SerializeField] private GameObject pathStart = null;
    [SerializeField] private GameObject pathEnd = null;

    private List<PathNode> validPath = new List<PathNode>();
    private bool validInit = true;

    public List<PathNode> path
    {
        get {return validPath;}
    }
    
    public bool hasValidPath()
    {
        return this.validPath.Count > 0;
    }

    void Awake()
    {
        if (pathStart == null || !pathStart.CompareTag(Utils.pathTag))
        {
            Debug.LogError("Path START point is INVALID");
            this.validInit = false;
        }

        if (pathEnd == null || !pathEnd.CompareTag(Utils.pathTag))
        {
            Debug.LogError("Path END point is INVALID");
            this.validInit = false;
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
        this.validPath.Clear();

        if (!this.validInit) return;
        
        // Set start node
        Cell_Update start = getEdgeCell(this.pathStart);
        Cell_Update end = getEdgeCell(this.pathEnd);

        if (start == null || end == null )
        {
            Debug.LogError("Could not FIND valid path START");
            return;
        }

         // Path edge does NOT connect
        if (!start.pathValue || !end.pathValue) return;

        // Initiate nodes
        Dictionary <Tuple<float, float>, PathNode> validNodes = new Dictionary <Tuple<float, float>, PathNode>();
        Queue <PathNode> newNodes = new Queue<PathNode> ();
        PathNode startNode = new PathNode(start, this.gameObject.transform.position);
        PathNode endNode = new PathNode(end, this.gameObject.transform.position);

        // Get valid paths
        newNodes.Enqueue(new PathNode(start, this.gameObject.transform.position));
        while (newNodes.Count != 0)
        {
            PathNode currentNode = newNodes.Dequeue();
            List<Cell_Update> neighborCells = currentNode.cellUpdate.findNeighborCells();

            foreach (Cell_Update cell in neighborCells)
            {
                PathNode neighborNode = new PathNode(cell, this.gameObject.transform.position);

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
        if (newPath != null) this.validPath = newPath;
    }

    void Start()
    {
        this.UpdatePath();

        // Find Map_Controller 
        Map_Controller controller = FindObjectOfType<Map_Controller>();
        if (controller == null)
        {
            Debug.LogError("Cound NOT find Map_Controller");
            return;
        }

        // Find Action_Inventory
        Action_Inventory inventory = controller.GetComponentInChildren<Action_Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Cound NOT find Action_Inventory");
            return;
        }

        // Add listeners to any performed action
        inventory.actionPerformed.AddListener(UpdatePath);
    }

}
