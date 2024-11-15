using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Tile_Rotation : MonoBehaviour
{
    [SerializeField] float rotationAmount = 90f;  // Rotation amount in degrees (fixed to 90)

    [SerializeField] private LayerMask targetLayer;                                 // "EDIT" bolo by vhodnÈ meniù Ëi chceme 90-180-270 ale to nieje prio
    [SerializeField] private string specificTag = "Policko";
    public GameObject requiredRotateObject;  // Optional: Specify which object needs to be held for rotation
   

    [Header("Layer Settings")]   //Editor Featurka 
    
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

        touchLayerMask = LayerMask.GetMask("Bunka_Layer");


    }


    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0) )
        {
            
            // Check if the required object is being held
            if (ObjectPickup.heldObject == requiredRotateObject)
            {
                
                // Perform a raycast at the mouse position
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, targetLayer);

                if (hit.collider != null)
                {
                    // Check if the object has the correct tag or meets other criteria
                    if (hit.collider.CompareTag(specificTag))
                    {
                        // Perform your desired action
                        Debug.Log($"Clicked on object with specific collider: {hit.collider.gameObject.name}");
                        this.HandleClick(hit.collider.gameObject);
                    }
                }
                
            }
            else
            {
                Debug.LogWarning("You are not holding the required object.");
            }
        }

       

    }




    private void HandleClick(GameObject clickedObject)
    {
        // Log the name of the clicked object
        Debug.Log($"Handling click for {clickedObject.name}");

        // Perform actions related to rotating and interacting with only the clicked object
        this.RotateChildrenAroundCenter(clickedObject);
        this.DetectInteractingObjectsForSpecificObject(clickedObject);  // Pass the clicked object to detect its specific interactions
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
            Debug.LogWarning("Clicked object is null. Aborting detection.");
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
        Debug.LogWarning($"Interacting objects count: {interactingObjects.Count}"); // [EDIT] toto sa vypÌöe XY kr·t podæa toho koæko kr·t je objekt s t˝mto scriptom na scÈne. Neviem preËo sa to tak robÌ ... nevadÌ to niËomu ale neviem ako to fixnuù :D 


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
           

            // Check if the object has the BunkaChange component kaûd· Bunka by mala maù tento script
            BunkaChange bunkaChange = obj.GetComponent<BunkaChange>();

            if (bunkaChange != null)
            {
                // Sem sa menia textury pre canvas pre konkrÈtnu bunku z loopu
                bunkaChange.Recalculate();
                
            }
            else
            {
                Debug.LogWarning("No BunkaChange component found on object: " + obj.name);
            }

            
        }
    }


}
