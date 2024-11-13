using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathDelete : MonoBehaviour
{
    
    [SerializeField] bool removePath = false;     
                                                  
    public GameObject requiredDeleteObject;  // Optional: Specify which object needs to be held for rotation

    [Header("Layer Settings")]   //Editor Featurka 

   
    private LayerMask touchLayerMask;
    // This will store the list of game objects touching child colliders and also child objects of the rotated parent
    private List<GameObject> interactingObjects = new List<GameObject>();



    [SerializeField] string targetTag = "Bunka";  // Tag filter to detect specific objects (Bunka)
    private Collider2D[] objectColliders;  // Reference to the object's collider

    // This will hold all the overlapping objects that we detect
    private List<GameObject> overlappingObjects = new List<GameObject>();


    private void Start()
    {
        
        // Ensure this object has a 2D collider (if not, add one)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("Collider2D not found! Please add a collider to this object for mouse detection.");

        }

        touchLayerMask = LayerMask.GetMask("Game_Map");

        objectColliders = GetComponents<Collider2D>();
    }

    private void OnMouseDown()
    {
        //Debug.Log("Object clicked: Checking rotation state.");

        // Only proceed if the required object is held and rotation hasn't happened yet
        if (ObjectPickup.heldObject == requiredDeleteObject && !removePath)
        {
            //Debug.Log("Object clicked: Rotating object now.");
            removePath = true;

            
            DetectOverlappingObjects();
        }
    }

    private void OnMouseUp()
    {
        //Debug.Log("Mouse button released: Resetting rotation flag.");
       removePath = false;
    }


    // Method to detect and add all colliders that are touching the child objects' side colliders, and include children of the rotated parent
    void DetectOverlappingObjects()
    {
        // Clear previous overlapping objects
        overlappingObjects.Clear();
        objectColliders = GetComponents<Collider2D>();

        if (objectColliders.Length > 0)
        {
            // Iterate through each collider attached to this object
            foreach (Collider2D collider in objectColliders)
            {
                // Get the center and size of the collider's bounds to use in the overlap check
                Vector2 center = collider.bounds.center;
                Vector2 size = collider.bounds.size;

                // Use Physics2D.OverlapBoxAll to find all colliders that overlap with the box defined by the collider's bounds
                Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(center, size, 0f); // 0 rotation

                // Check all detected overlapping colliders
                foreach (Collider2D overlap in overlappingColliders)
                {
                    if (overlap != null && overlap.gameObject != gameObject)  // Ensure we don't detect the object itself
                    {
                        // Check if the object has the tag "Bunka"
                        if (overlap.CompareTag(targetTag))
                        {
                            // Add the object to the list if it has the "Bunka" tag
                            if (!overlappingObjects.Contains(overlap.gameObject))
                            {
                                overlappingObjects.Add(overlap.gameObject);
                            }
                        }
                    }
                }
            }

            // Optionally, log the overlapping objects
            foreach (GameObject obj in overlappingObjects)
            {
                Debug.Log("Overlapping object with tag 'Bunka': " + obj.name);
            }
        }
        else
        {
            Debug.LogWarning("No Collider2D found on this object.");
        }
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
                Debug.LogWarning("No BunkaChangeeees component found on object: " + obj.name);
            }


        }
    }


}
