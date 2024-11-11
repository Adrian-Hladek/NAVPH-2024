using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tile_Rotation : MonoBehaviour
{
    [SerializeField] float rotationAmount = 90f;  // Rotation amount in degrees (fixed to 90)
    [SerializeField] bool hasRotated = false;     // Flag to ensure rotation happens only once per click

    public GameObject requiredHeldObject;  // Optional: Specify which object needs to be held for rotation

    [Header("Layer Settings")]
    // Exposed Layer Mask to specify which layer to check against
    [SerializeField] string targetTag = "Bunka";  // Target tag to look for
    private LayerMask touchLayerMask;
    // This will store the list of game objects touching child colliders and also child objects of the rotated parent
    private List<GameObject> interactingObjects = new List<GameObject>();

    private void Start()
    {
        // Ensure this object has a 2D collider (if not, add one)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("Collider2D not found! Please add a collider to this object for mouse detection.");
           
        }

        touchLayerMask = LayerMask.GetMask("Game_Map");


    }

    private void OnMouseDown()
    {
        //Debug.Log("Object clicked: Checking rotation state.");

        // Only proceed if the required object is held and rotation hasn't happened yet
        if (ObjectPickup.heldObject == requiredHeldObject && !hasRotated)
        {
            //Debug.Log("Object clicked: Rotating object now.");
            hasRotated = true;
            RotateChildrenAroundCenter();
            DetectInteractingObjects();  // Detect interacting objects after rotation
        }
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse button released: Resetting rotation flag.");
        hasRotated = false;
    }

    // Method to rotate all child objects around their calculated center
    void RotateChildrenAroundCenter()
    {
        Vector3 center = CalculateCenterOfChildren();

        foreach (Transform child in transform)
        {
            RotateAroundPoint(child, center);
        }
    }

    // Method to calculate the center point by averaging the positions of all child objects
    Vector3 CalculateCenterOfChildren()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Transform child in transform)
        {
            sum += child.position;
            count++;
        }

        if (count == 0)
        {
            //Debug.LogWarning("No children found! Using parent's position as the center.");
            return transform.position;
        }

        return sum / count;
    }

    // Method to rotate a specific child around a given point (center)
    void RotateAroundPoint(Transform child, Vector3 center)
    {
        Vector3 direction = child.position - center;
        direction = Quaternion.Euler(0, 0, rotationAmount) * direction;
        child.position = center + direction;

        //Debug.Log("Rotating child: " + child.name + " around point " + center);
    }

    // Method to detect and add all colliders that are touching the child objects' side colliders, and include children of the rotated parent
    void DetectInteractingObjects()
    {
        List<GameObject> interacting = new List<GameObject>();

        // Iterate through each child of the rotated object
        foreach (Transform child in transform)
        {
            // First, add the child object itself to the interacting list (since you want to include them)
            if (!interacting.Contains(child.gameObject))
            {
                interacting.Add(child.gameObject);
                //Debug.Log("Added child object: " + child.name);
            }

            // Get all the colliders attached to the current child (assuming there may be multiple colliders)
            Collider2D[] childColliders = child.GetComponents<Collider2D>();

            // Iterate through each collider attached to the child
            foreach (Collider2D collider in childColliders)
            {
                // Debugging: Log collider bounds to check if the area is correct
                //Debug.Log($"Checking for touching colliders for child: {child.name}, Collider Bounds: {collider.bounds}");

                // Use OverlapBoxAll to detect all colliders that intersect with this one
                Collider2D[] touching = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0, touchLayerMask);
                


                //Debug.Log(touching);
                // Loop through each collider detected by OverlapBoxAll
                foreach (Collider2D col in touching)
                {
                    if (col != null && col.gameObject != child.gameObject)  // Ensure we don't add the child itself
                    {
                        // Log the detected colliders
                        //Debug.Log("Detected collider: " + col.name);

                        // Only add objects with the specified tag
                        if (col.CompareTag(targetTag))
                        {
                            // Avoid adding duplicates to the list
                            if (!interacting.Contains(col.gameObject))
                            {
                                interacting.Add(col.gameObject);
                                //Debug.Log("Touching tagged object added: " + col.name);
                            }
                        }
                    }
                }
            }
        }

        // Store the result in the parent class's array of interacting objects
        interactingObjects = interacting;

        // Print the number of interacting objects in the array
        //Debug.Log("Total number of interacting objects: " + interactingObjects.Count);

       

        // Call your separate function to perform operations on the interacting objects
        PerformOperationsOnInteractingObjects();
    }

    void PerformOperationsOnInteractingObjects()
    {
        // Iterate through each object in the interacting objects list
        foreach (GameObject obj in interactingObjects)
        {
            // Perform your desired operation, for example:
            //Debug.Log("Performing operation on: " + obj.name);

            // Check if the object has the BunkaChange component
            BunkaChange bunkaChange = obj.GetComponent<BunkaChange>();

            if (bunkaChange != null)
            {
                // Call the Recalculate method on the BunkaChange component
                bunkaChange.Recalculate();
                //Debug.Log("Recalculated BunkaChange for: " + obj.name);
            }
            else
            {
                Debug.LogWarning("No BunkaChange component found on object: " + obj.name);
            }

            // Your custom logic goes here (after recalculating)
        }
    }


}
