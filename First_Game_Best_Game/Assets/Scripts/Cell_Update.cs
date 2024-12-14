using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;

public class Cell_Update : MonoBehaviour
{
    [SerializeField] private bool canHavePath = false;
    private bool hasPath;
    private bool hasTurret;

    private SpriteRenderer spriteRenderer;

    public bool possiblePath
    {
        get { return canHavePath; }
    }

    public bool pathValue
    {
        get { return hasPath; }
        set { hasPath = value; }
    }

    public bool turretValue
    {
        get { return hasPath; }
        set { hasPath = value; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component NOT found on {this.gameObject.name}");
            return;
        }

        // Set hasPath 
        if (Utils.spriteEmptyTile == spriteRenderer.sprite.name) hasPath = false;
        else hasPath = true;
    }

    private void Start()
    {
        Recalculate();
    }

    public List<Collider2D> getChildColliders()
    {
        List<Collider2D> childColliders = new List<Collider2D>();

        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            if (collider.gameObject != this.gameObject) childColliders.Add(collider);
       
        return childColliders;
    }

    public void Recalculate()
    {
        bool hasRight = false;
        bool hasLeft = false;
        bool hasUpper = false;
        bool hasBottom = false;

        List<GameObject> detectedObjects = new List<GameObject>();
        List<Collider2D> childColliders = this.getChildColliders();

        foreach (Collider2D collider in childColliders)
        {
            Collider2D[] touchingObjects = new Collider2D[10];
            int overlapCount = Physics2D.OverlapCollider(collider, new ContactFilter2D().NoFilter(), touchingObjects);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D collision = touchingObjects[i];
                
                if (collision.gameObject == this.gameObject) continue;

                if (collision.CompareTag(Utils.cellTag) && !detectedObjects.Contains(collision.gameObject))
                {
                    string position = GetRelativePosition(collision);

                    Cell_Update foundCell = collision.GetComponent<Cell_Update>();
                    if (foundCell != null && foundCell.canHavePath && foundCell.hasPath)
                    {
                        switch (position)
                        {
                            case "Up":
                                hasUpper = true;
                                break;
                            case "Down":
                                hasBottom = true;
                                break;
                            case "Left":
                                hasLeft = true;
                                break;
                            case "Right":
                                hasRight = true;
                                break;
                        }
                    }

                    detectedObjects.Add(collision.gameObject);
                }
                else if (collision.CompareTag(Utils.pathTag))
                {
                    string position = GetRelativePosition(collision);

                    switch (position)
                    {
                        case "Up":
                            hasUpper = true;
                            break;
                        case "Down":
                            hasBottom = true;
                            break;
                        case "Left":
                            hasLeft = true;
                            break;
                        case "Right":
                            hasRight = true;
                            break;
                    }
                }
            }
        }
        
        if (detectedObjects.Count > 4) 
            Debug.LogError($"More than 4 objects detected: {detectedObjects.Count} objects found.");

        // Set sprite
        string spritePath = Utils.getCellSprite(hasPath, hasTurret, canHavePath, hasRight, hasLeft, hasUpper, hasBottom);
        spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
    }

    // Get all related objects
    public GameObject[] GetRelatedObjects(GameObject obj)
    {
        // Return an empty array if input is invalid
        if (obj == null)
        {
            Debug.LogError($"GameObject {obj.name} is null, returning an empty array");
            return new GameObject[0];
        }

        List<GameObject> interactingObjects = new List<GameObject>{obj};

        // Process colliders for this child
        Collider2D[] childColliders = obj.GetComponents<Collider2D>();
       
        foreach (Collider2D collider in childColliders)
        {
            // Use OverlapBoxAll to detect colliders overlapping with this collider
            Collider2D[] collisions = Physics2D.OverlapBoxAll(
                collider.bounds.center,
                collider.bounds.size,
                0f, // No rotation for 2D
                LayerMask.GetMask(Utils.cellLayer)
            );

            foreach (Collider2D colider in collisions)
            {
                // Prevent adding the input object and match specific tag
                if ( colider != null 
                    //&& colider.gameObject != obj 
                    && colider.CompareTag(Utils.cellTag)
                    && !interactingObjects.Contains(colider.gameObject)
                ) 
                {
                    interactingObjects.Add(colider.gameObject);
                }
            }
        }

        Debug.Log($"Objects detected: {interactingObjects.Count} objects found.");

        // Convert the list of interacting objects to an array and return it
        return interactingObjects.ToArray();
    }

    // Get relative position of 
    private string GetRelativePosition(Collider2D detectedCollider)
    {
        Vector3 mainObjectPosition = transform.position;
        Vector3 detectedPosition = detectedCollider.transform.position;

        bool isYClose = Mathf.Abs(detectedPosition.y - mainObjectPosition.y) < Utils.epsilon;
        bool isXClose = Mathf.Abs(detectedPosition.x - mainObjectPosition.x) < Utils.epsilon;

        if (isYClose && isXClose) return "Center";
        
        if (detectedPosition.y > mainObjectPosition.y && !isYClose) return "Up";

        if (detectedPosition.y < mainObjectPosition.y && !isYClose) return "Down";
        
        if (detectedPosition.x > mainObjectPosition.x && !isXClose) return "Right";
        
        if (detectedPosition.x < mainObjectPosition.x && !isXClose) return "Left";
        
        return "Center";
    }

    // Calls Recalculate on all coliding cells + itself
    public void UpdateNearbyCells()
    {
        GameObject[] relatedObjects = GetRelatedObjects(this.gameObject);

        if (relatedObjects.Length == 0)
        {
            Debug.LogError($"No related objects found for {this.gameObject.name}");
            return;
        }

        foreach (GameObject obj in relatedObjects)
        {
            Cell_Update otherCell = obj.GetComponent<Cell_Update>();
            
            if (otherCell != null) otherCell.Recalculate();
            else
            {
                Debug.LogError($"Related object {obj.name} does NOT have Cell_Update component");
            }
        }
    }

    // Find all neighboring cells that have path
    public List<Cell_Update> findNeighborCells()
    {
        List <Cell_Update> detectedCells = new List <Cell_Update>();
        List<Collider2D> childColliders = this.getChildColliders();

        foreach (Collider2D collider in childColliders)
        {
            Collider2D[] touchingObjects = new Collider2D[10];

            int overlapCount = Physics2D.OverlapCollider(collider, new ContactFilter2D().NoFilter(), touchingObjects);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D col = touchingObjects[i];

                if (!col.CompareTag(Utils.cellTag) || col.gameObject == this.gameObject) continue;

                Cell_Update newCell = col.GetComponent<Cell_Update>();
                if (newCell != null && newCell.hasPath) detectedCells.Add(newCell);
            }
        }

        return detectedCells;
    }


}
