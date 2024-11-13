using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover_Highlight_Big : MonoBehaviour
{
    private Collider2D firstCollider;

    [SerializeField]
    private GameObject highlightChild; // Reference to the child object used for highlighting
    public GameObject requiredValidObject;

    private void Awake()
    {
        // Automatically get the first Collider2D on this GameObject
        firstCollider = GetComponent<Collider2D>();

        // Ensure the highlight child object is assigned and initially inactive
        if (highlightChild != null)
        {
            highlightChild.SetActive(false); // Hide initially
        }
        else
        {
            Debug.LogWarning("Highlight child object is not assigned.");
        }
    }

    /*
    private void OnMouseEnter()
    {
        // Enable the highlight when the mouse enters the collider
        if (ObjectPickup.heldObject == requiredValidObject )
        {

            if (highlightChild != null)
            {
                highlightChild.SetActive(true);
            }
        }

    }

    private void OnMouseExit()
    {
        // Disable the highlight when the mouse exits the collider
        if (highlightChild != null)
        {
            highlightChild.SetActive(false);
        }
    }
    */

    private void Update()
    {
        // Check if no object is held or the held object is not the requiredInvalidObject
        if (ObjectPickup.heldObject == null || ObjectPickup.heldObject == requiredValidObject)
        {
            // Check if the mouse is over the first collider
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (firstCollider != null && firstCollider.OverlapPoint(mousePosition))
            {
                // Enable the highlight if the mouse is over the first collider
                if (!highlightChild.activeSelf)
                {
                    highlightChild.SetActive(true);
                }
            }
            else
            {
                // Disable the highlight if the mouse is not over the first collider
                if (highlightChild.activeSelf)
                {
                    highlightChild.SetActive(false);
                }
            }
        }
        else
        {
            // Ensure highlight is disabled if the held object condition is not met
            if (highlightChild.activeSelf)
            {
                highlightChild.SetActive(false);
            }
        }
    }
}
