using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Tile_Rotation : MonoBehaviour
{
    [SerializeField] string centerChildName ; // The name of the child to rotate around
    [SerializeField] float rotationAmount = 90f; // Rotation amount in degrees (fixed to 90)
    [SerializeField] bool hasRotated = false; // Flag to ensure rotation happens only once per click


    // Optional: Specify which object needs to be held for rotation (assign in Inspector)
    public GameObject requiredHeldObject;


    private void Start()
    {
        // Ensure this object has a 2D collider (if not, add one)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("Collider2D not found! Please add a collider to this object for mouse detection.");
        }
    }

    // Called when the object is clicked
    private void OnMouseDown()
    {
        Debug.Log("Object clicked: Checking rotation state.");

        // Check if any ObjectPickup instances exist in the scene
        ObjectPickup[] allObjectPickups = FindObjectsOfType<ObjectPickup>();

        // If no ObjectPickup exists, we don't need to proceed
        if (allObjectPickups.Length == 0)
        {
            Debug.Log("No ObjectPickup instances found.");
            return; // Exit if no ObjectPickup objects exist
        }

        // Only proceed if we have the required object held
        // We assume ObjectPickup.heldObject is a reference to the currently held object
        if (ObjectPickup.heldObject == requiredHeldObject && !hasRotated)
        {
            Debug.Log("Object clicked: Rotating object now.");
            hasRotated = true;
            RotateChildrenAroundNamedCenter();
        }
    }

    // Reset rotation flag when mouse button is released, allowing rotation on the next click
    private void OnMouseUp()
    {
        Debug.Log("Mouse button released: Resetting rotation flag.");
        hasRotated = false;
    }

    // Method to rotate the children of the object around the center (middle child or calculated center)
    void RotateChildrenAroundNamedCenter()
    {
        Transform centerChild = transform.Find(centerChildName);
        Vector3 center = centerChild != null ? centerChild.position : CalculateCenterOfChildren();

        foreach (Transform child in transform)
        {
            if (child != centerChild)  // Don't rotate the center itself
            {
                RotateAroundPoint(child, center);
            }
        }
    }


    // Method to calculate the center point by averaging the positions of all child objects
    Vector3 CalculateCenterOfChildren()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        // Add the position of each child to the sum
        foreach (Transform child in transform)
        {
            sum += child.position;
            count++;
        }

        // If there are no children, return the current position of the parent
        if (count == 0)
        {
            Debug.LogWarning("No children found! Using parent's position as the center.");
            return transform.position;
        }

        // Calculate the average position (center)
        return sum / count;
    }

    // Method to rotate a specific child around a given point (center)
    void RotateAroundPoint(Transform child, Vector3 center)
    {
        // Get the direction from the center to the child in the 2D plane (Z-axis rotation)
        Vector3 direction = child.position - center;

        // Apply a fixed 90 degree rotation to the direction vector (rotate it around the Z-axis for 2D)
        direction = Quaternion.Euler(0, 0, rotationAmount) * direction;

        // Update the child's position based on the new direction
        child.position = center + direction;

        // Debug log for each child being rotated
        Debug.Log("Rotating child: " + child.name + " around point " + center);
    }
}
