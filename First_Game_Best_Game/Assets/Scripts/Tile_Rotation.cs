using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Rotation : MonoBehaviour
{
    public string centerChildName = "MiddleChild"; // The name of the child to rotate around
    public float rotationAmount = 90f; // Rotation amount in degrees (fixed to 90)
    private bool hasRotated = false; // Flag to ensure rotation happens only once per hover

    private void Start()
    {
        // Ensure this object has a 2D collider (if not, add one)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("Collider2D not found! Please add a collider to this object for mouse detection.");
        }
    }

    // Called when the mouse enters the collider of the object
    private void OnMouseEnter()
    {
        Debug.Log("Mouse entered: Rot");

        // Only rotate if we haven't already rotated during this hover
        if (!hasRotated)
        {
            Debug.Log("Mouse entered: Rotating object now.");
            hasRotated = true;
            RotateChildrenAroundNamedCenter();
        }
    }

    // Called when the mouse exits the collider of the object
    private void OnMouseExit()
    {
        // Reset the flag so the object can be rotated again on next hover if needed
        Debug.Log("Mouse exited: Resetting rotation flag.");
        hasRotated = false;
    }

    // Method to rotate the children of the object around the center (middle child or calculated center)
    void RotateChildrenAroundNamedCenter()
    {
        // Try to find the center child by its name
        Transform centerChild = transform.Find(centerChildName);

        // If the center child is found, use its position as the center for rotation
        Vector3 center = centerChild != null ? centerChild.position : CalculateCenterOfChildren();

        // Rotate each child around the calculated center of the grid
        foreach (Transform child in transform)
        {
            // Skip the center child itself to avoid rotating it
            if (child != centerChild)
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
