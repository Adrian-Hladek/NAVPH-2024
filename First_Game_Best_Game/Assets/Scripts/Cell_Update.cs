using System.Collections.Generic;
using UnityEngine;

public class Cell_Update : MonoBehaviour
{
    [SerializeField] private bool canHavePath = false;
    bool hasPath;

    // odstrani� serialized je to iba aby som videl zmenu v editore
    [SerializeField] private bool hasTower;
    [SerializeField] private GameObject towerPrefab;
    //Action s anedá v inspectore zobraziť by default  :( sad lebo abstract class
    private Action towerAction;

    int towerOrder = 20;
    SpriteRenderer spriteRenderer;

    

    public bool CanHavePath
    {
        get { return canHavePath; }
    }

    public bool HasPath
    {
        get { return hasPath; }
        set { hasPath = value; }
    }

    public bool HasTower
    {
        get { return hasTower; }
        set { hasTower = value; }
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

    void Start()
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
                if (collision.gameObject == this.gameObject || detectedObjects.Contains(collision.gameObject)) continue;

                if (collision.CompareTag(Utils.cellTag) || collision.CompareTag(Utils.pathTag))
                {
                    Direction direction = Utils.DirectionBetweenPoints(this.transform.position, collision.transform.position);

                    Cell_Update foundCell = collision.GetComponent<Cell_Update>();
                    if (collision.CompareTag(Utils.pathTag) || (foundCell != null && foundCell.canHavePath && foundCell.hasPath))
                    {
                        switch (direction)
                        {
                            case Direction.Up:
                                hasUpper = true;
                                break;
                            case Direction.Down:
                                hasBottom = true;
                                break;
                            case Direction.Left:
                                hasLeft = true;
                                break;
                            case Direction.Right:
                                hasRight = true;
                                break;
                        }
                    }

                    detectedObjects.Add(collision.gameObject);
                }
            }
        }
        
        if (detectedObjects.Count > 5) Debug.LogError($"More than 5 objects detected: {detectedObjects.Count} objects found.");

        // Set sprite
        string spritePath = Utils.getCellSprite(hasPath, hasTower, canHavePath, hasRight, hasLeft, hasUpper, hasBottom);
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

        //Debug.Log($"Objects detected: {interactingObjects.Count} objects found.");

        // Convert the list of interacting objects to an array and return it
        return interactingObjects.ToArray();
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

    public void AddTower(int towerType, Action action)
    {

        towerAction = action;

        if (towerPrefab == null)
        {
            Debug.LogError("Tower prefab is not assigned in the Inspector.");
            return;
        }

        GameObject child = Instantiate(towerPrefab, transform);
        Tower_Update towerStats = child.GetComponent<Tower_Update>();
        if (towerStats != null)
        {
            Debug.Log(towerStats.name);
            towerStats.TowerSetup(towerType);
        }


        // Optionally, log the successful addition
        Debug.Log($"Prefab '{towerPrefab.name}' instantiated as a child.");
    }

    public void RemoveTower()
    {
        if (transform.childCount == 0)
        {
            Debug.LogWarning("No towers to remove.");
            return;
        }

        // Find the first child (the most recently added tower, if following AddTower logic) Toto by malo byť simple ale bulletproof pretože Na bunke s abude pridavať iba 1 objekt max
        Transform child = transform.GetChild(transform.childCount - 1);

        // Log the name of the child being removed
        Debug.Log($"Removing tower: {child.name}");
        towerAction.AddAction(1);

        // Destroy the child GameObject
        Destroy(child.gameObject);

        // Optionally, log the successful removal
        Debug.Log("Tower removed successfully.");





    }

}
