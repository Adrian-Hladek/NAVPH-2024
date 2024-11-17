using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class PathCreation : MonoBehaviour
{



   
    [SerializeField] private LayerMask targetLayer;                                 // "EDIT" bolo by vhodnÈ meniù Ëi chceme 90-180-270 ale to nieje prio
    [SerializeField] private string BunkaTag = "Bunka";
    [SerializeField] public GameObject requiredCreateObject;  // Optional: Specify which object needs to be held for rotation


    [Header("Layer Settings")]   //Editor Featurka 

   
    private LayerMask touchLayerMask;
    // This will store the list of game objects touching child colliders and also child objects of the rotated parent
    private List<GameObject> interactingObjects = new List<GameObject>();

    

    // Update is called once per frame
    void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {

            // Check if the required object is being held
            if (ObjectPickup.heldObject == requiredCreateObject)
            {

                // Perform a raycast at the mouse position
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, targetLayer);

                if (hit.collider != null)
                {

                    BunkaChange bunkaChange = hit.collider.gameObject.GetComponent<BunkaChange>();

                    if (bunkaChange != null && bunkaChange.IsPathValue && !bunkaChange.HasPathValue)
                    {


                        // Check if the object has the correct tag or meets other criteria
                        if (hit.collider.CompareTag(BunkaTag))
                        {
                            // Perform your desired action
                            Debug.Log($"Clicked on object with specific collider: {hit.collider.gameObject.name}");
                            this.HandleClick(hit.collider.gameObject);
                        }

                    }
                }
            }
            else
            {
             //   Debug.LogWarning("You are not holding the required object.");
            }
        }



    }


    private void HandleClick(GameObject clickedObject)
    {
        // Log the name of the clicked object
        Debug.LogWarning($"Handling click for {clickedObject.name}");

        // Perform actions related to rotating and interacting with only the clicked object
        
        
        AddPathToBunka(clickedObject);
        
    }


    private void AddPathToBunka(GameObject Bunka)
    {
        if (Bunka == null)
        {
            Debug.LogError("Bunka object is null. Aborting AddPathToBunka.");
            return;
        }

        Debug.LogWarning("Bunka clicked");
        BunkaChange bunkaChange = Bunka.GetComponent<BunkaChange>();
        if (bunkaChange != null)
        {
            Debug.Log($"Adding path to Bunka. Current HasPathValue: {bunkaChange.IsPathValue}");
            bunkaChange.HasPathValue = true; // Set the property value

            GameObject[] relatedObjects = bunkaChange.GetRelatedObjects(Bunka);

            if (relatedObjects == null || relatedObjects.Length == 0)
            {
                Debug.LogWarning("No related objects found.");
                return;
            }

            Debug.LogWarning($"Number of related objects: {relatedObjects.Length}");

            foreach (GameObject obj in relatedObjects)
            {
                BunkaChange objBunka = obj.GetComponent<BunkaChange>();
                if (objBunka != null)
                {
                    Debug.Log($"Recalculating Bunka: {obj.name}");
                    objBunka.Recalculate();
                }
                else
                {
                    Debug.LogWarning($"Related object {obj.name} does not have a BunkaChange component.");
                }
            }
        }
        else
        {
            Debug.LogError("The Bunka object does not have a BunkaChange component!");
        }
    }

}
