using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tile_Rotation : MonoBehaviour
{
    [SerializeField] float rotationAmount = 90f; // Rotation amount in degrees (fixed to 90)
    [SerializeField] bool hasRotated = false;    // Flag to ensure rotation happens only once per click

    public GameObject requiredHeldObject;  // Optional: Specify which object needs to be held for rotation

    private void Start()
    {
        // Ensure this object has a 2D collider (if not, add one)
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("Collider2D not found! Please add a collider to this object for mouse detection.");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Object clicked: Checking rotation state.");

        // Only proceed if the required object is held and rotation hasn't happened yet
        if (ObjectPickup.heldObject == requiredHeldObject && !hasRotated)
        {
            Debug.Log("Object clicked: Rotating object now.");
            hasRotated = true;
            RotateChildrenAroundCenter();
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse button released: Resetting rotation flag.");
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
            Debug.LogWarning("No children found! Using parent's position as the center.");
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

        Debug.Log("Rotating child: " + child.name + " around point " + center);
    }
}
