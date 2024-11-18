using System.Collections.Generic;
using UnityEngine;

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

    public void Recalculate()
    {
        bool hasRight = false;
        bool hasLeft = false;
        bool hasUpper = false;
        bool hasBottom = false;
        List<GameObject> detectedObjects = new List<GameObject>();

        Physics2D.SyncTransforms();

        // Get all colliders in children, but filter out the ones directly on this GameObject
        List<Collider2D> childColliders = new List<Collider2D>();

        // Only add colliders from child objects
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            if (collider.gameObject != this.gameObject)  
            {
                childColliders.Add(collider);
            }
        }

        foreach (Collider2D mainCollider in childColliders)
        {
            Collider2D[] touchingObjects = new Collider2D[10];
            int overlapCount = Physics2D.OverlapCollider(mainCollider, new ContactFilter2D().NoFilter(), touchingObjects);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D col = touchingObjects[i];

                if (col.gameObject == this.gameObject)
                    continue;

                if (col.CompareTag(Utils.cellTag) && !detectedObjects.Contains(col.gameObject))
                {
                    string position = GetRelativePosition(mainCollider, col);
                    //Debug.Log($"Detected object: {col.gameObject.name} at position: {position}");

                    Cell_Update bunkaProps = col.GetComponent<Cell_Update>();
                    if (bunkaProps != null && bunkaProps.canHavePath && bunkaProps.hasPath)
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

                    detectedObjects.Add(col.gameObject);
                }
            }
        }

        if (detectedObjects.Count > 4)
        {
            Debug.LogError($"More than 4 objects detected: {detectedObjects.Count} objects found.");
        }

        string spritePath = Utils.getCellSprite(hasPath, hasTurret, canHavePath, hasRight, hasLeft, hasUpper, hasBottom);
        spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
    }

    public GameObject[] GetRelatedObjects(GameObject obj)
    {
        // Return an empty array if input is invalid
        if (obj == null)
        {
            Debug.LogWarning($"GameObject {obj.name} is null, returning an empty array");
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
                if (
                    colider != null 
                    && colider.gameObject != obj 
                    && colider.CompareTag(Utils.cellTag)
                    && !interactingObjects.Contains(colider.gameObject)
                )
                {
                    interactingObjects.Add(colider.gameObject);
                }
            }
        }

        Debug.LogWarning($"Objects detected: {interactingObjects.Count} objects found.");

        // Convert the list of interacting objects to an array and return it
        return interactingObjects.ToArray();
    }

    private string GetRelativePosition(Collider2D mainCollider, Collider2D detectedCollider)
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
            Debug.Log($"UPDATING {obj.name}");
            
            Cell_Update otherCell = obj.GetComponent<Cell_Update>();
            
            if (otherCell != null) otherCell.Recalculate();
            else
            {
                Debug.LogError($"Related object {obj.name} does NOT have Cell_Update component");
            }
        }
    }


}
