using System.Collections.Generic;
using UnityEngine;


// TODO - remove file, but extract usefull functions to Utils.cs 
public class Tile_Rotation
{
    [SerializeField] float rotationAmount = 90f;  // Rotation amount in degrees
    [SerializeField] string rotationDirection = "left"; // TODO

    [Header("Layer Settings")]   //Editor Featurka 
    
    [SerializeField] string targetTag = "Cell";  // Target tag to look for
    private LayerMask touchLayerMask;

    // This will store the list of game objects touching child colliders and also child objects of the rotated parent
    private List<GameObject> interactingObjects = new List<GameObject>();

   
    private void HandleClick(GameObject clickedObject)
    {
        // Log the name of the clicked object
        Debug.Log($"Handling click for {clickedObject.name}");

        // Perform actions related to rotating and interacting with only the clicked object
        RotateChildrenAroundCenter(clickedObject);
        DetectInteractingObjectsForSpecificObject(clickedObject);  // Pass the clicked object to detect its specific interactions
    }

    // Method to rotate all child objects around their calculated center
    void RotateChildrenAroundCenter(GameObject clickedObject)
    {
        // Calculate the center of only the clicked object's children
        Vector3 center = CalculateCenterOfChildren(clickedObject);

        // Rotate only the clicked object's children around the center
        foreach (Transform child in clickedObject.transform)
        {
            RotateAroundPoint(child, center);
        }
    }

    // Method to calculate the center point by averaging the positions of all child objects
    Vector3 CalculateCenterOfChildren(GameObject clickedObject)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        // Only iterate over the children of the clicked object
        foreach (Transform child in clickedObject.transform)
        {
            sum += child.position;
            count++;
        }

        if (count == 0)
        {
            // Return the clicked object's position if it has no children
            return clickedObject.transform.position;
        }

        return sum / count;
    }

    void RotateAroundPoint(Transform child, Vector3 center)
    {
        Vector3 direction = child.position - center;
        direction = Quaternion.Euler(0, 0, rotationAmount) * direction;
        child.position = center + direction;
    }

    // Method to detect and add all colliders that are touching the child objects' side colliders, and include children of the rotated parent
    private void DetectInteractingObjectsForSpecificObject(GameObject clickedObject)
    {
        List<GameObject> interacting = new List<GameObject>();

        // Ensure the clicked object is valid
        if (clickedObject == null)
        {
            Debug.LogError("Clicked object is null. Aborting detection.");
            return;
        }

        foreach (Transform child in clickedObject.transform)
        {
            // Check if the child is not null
            if (child == null)
            {
                Debug.LogWarning("Encountered a null child. Skipping.");
                continue;
            }

            // Check if the child's tag matches the specific tag
            if (child.CompareTag(targetTag))
            {
                Debug.Log($"Child {child.name} matches the specific tag: {targetTag}");
                

                if (!interacting.Contains(child.gameObject))
                {
                    interacting.Add(child.gameObject);
                    
                }

                // Process colliders for this child
                Collider2D[] childColliders = child.GetComponents<Collider2D>();
                foreach (Collider2D collider in childColliders)
                {
                    // Use OverlapBoxAll to detect colliders overlapping with this collider
                    Collider2D[] touching = Physics2D.OverlapBoxAll(
                        collider.bounds.center,
                        collider.bounds.size,
                        0f, // No rotation needed for 2D
                        touchLayerMask
                    );

                    foreach (Collider2D col in touching)
                    {
                        if (col != null &&
                            col.gameObject != clickedObject &&
                            col.gameObject != child.gameObject &&
                            col.CompareTag(targetTag))
                        {
                            if (!interacting.Contains(col.gameObject))
                            {
                                interacting.Add(col.gameObject);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"Child {child.name} does not match the specific tag: {targetTag}");
            }
        }

        // Store the interacting objects in the parent class list
        interactingObjects = interacting;

        // Log the count of interacting objects for debugging
        Debug.LogWarning($"Interacting objects count: {interactingObjects.Count}"); // [EDIT] toto sa vyp�e XY kr�t pod�a toho ko�ko kr�t je objekt s t�mto scriptom na sc�ne. Neviem pre�o sa to tak rob� ... nevad� to ni�omu ale neviem ako to fixnu� :D 


        foreach (GameObject obj in interactingObjects)
        {
            Debug.Log($"Interacting object: {obj.name}");
        }


        // Perform operations on the detected interacting objects
        PerformOperationsOnInteractingObjects();
    }

    void PerformOperationsOnInteractingObjects()
    {
        
        foreach (GameObject obj in interactingObjects)
        {
            // Check if the object has the BunkaChange component ka�d� Bunka by mala ma� tento script
            Cell_Update bunkaChange = obj.GetComponent<Cell_Update>();

            if (bunkaChange != null)
            {
                // Sem sa menia textury pre canvas pre konkr�tnu bunku z loopu
                bunkaChange.Recalculate();
                
            }
            else
            {
                Debug.LogWarning("No BunkaChange component found on object: " + obj.name);
            }

            
        }
    }


}
