using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Tile_Rotation : MonoBehaviour
{
    [SerializeField] float rotationAmount = 90f;  // Rotation amount in degrees (fixed to 90)
    [SerializeField] bool hasRotated = false;     // Flag to ensure rotation happens only once per click
                                                  // "EDIT" bolo by vhodnÈ meniù Ëi chceme 90-180-270 ale to nieje prio

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

    private void OnMouseDown()
    {
        //Debug.Log("Object clicked: Checking rotation state.");

        // Only proceed if the required object is held and rotation hasn't happened yet
        if (ObjectPickup.heldObject == requiredRotateObject && !hasRotated)
        {
            //Debug.Log("Object clicked: Rotating object now.");
            hasRotated = true;
            RotateChildrenAroundCenter();
            DetectInteractingObjects();  
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
        // "EDIT" Ai Generated funkcia robÌ priemer pozÌcii 
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (Transform child in transform)
        {
            sum += child.position;
            count++;
        }

        if (count == 0)
        {
            
            return transform.position;
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
    void DetectInteractingObjects()
    {
        List<GameObject> interacting = new List<GameObject>();

        //V liste by malo byù teoreticky 9-21 objektov na konci funkcie, Prakticky moûe byù 12-21 re·lne bude ale 15-21(RohovÈ pole, ktorÈ sa dot˝ka 2 œalöÌch) 
        // Iterate through each child of the rotated object
        foreach (Transform child in transform)
        {
            // add the child object itself to the interacting list 
            {
                

                BunkaChange bunkaChange = child.GetComponent<BunkaChange>();

                if (bunkaChange != null)  // If the component exists on the child
                {
                    // Add the child object to the interacting list if it has the BunkaChange component
                    interacting.Add(child.gameObject);
                }




            }

            // Get all the colliders attached to the current child (assuming there may be multiple colliders -> s˙ pr·ve 4, jeden na kaûdej strane)
            Collider2D[] childColliders = child.GetComponents<Collider2D>();

            // Iterate through each collider attached to the child
            foreach (Collider2D collider in childColliders)
            {
                
                // Use OverlapBoxAll to detect all colliders that intersect with this one
                Collider2D[] touching = Physics2D.OverlapBoxAll(collider.bounds.center, collider.bounds.size, 0, touchLayerMask);
                


                
                // Loop through each collider detected by OverlapBoxAll
                foreach (Collider2D col in touching)
                {
                    if (col != null && col.gameObject != child.gameObject)  // Ensure we don't add the child itself 
                    {
                   
                        // Only add objects with the specified tag (Moûeme sa dot˝kaù ötartovnÈho alebo koncovÈho polÌËka, ktorÈ je nemennÈ)
                        if (col.CompareTag(targetTag))
                        {
                            // Avoid adding duplicates to the list
                            if (!interacting.Contains(col.gameObject))
                            {
                                interacting.Add(col.gameObject);
                                
                            }
                        }
                    }
                }
            }
        }

        // Store the result in the parent class's array of interacting objects
        interactingObjects = interacting;

        Debug.LogWarning(interactingObjects.Count);
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
