using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    private Vector3 originalPosition;  // Original position of the object to return to
    private bool isPickedUp = false;   // Whether the object is being dragged
    private Camera mainCamera;         // Reference to the main camera
    private Vector3 offset;            // Offset between mouse position and object's position

    private Collider2D objectCollider; // Reference to the Collider2D component

    private void Start()
    {
        // Store the original position of the object when the script starts
        originalPosition = transform.position;
        mainCamera = Camera.main;
        objectCollider = GetComponent<Collider2D>(); // Get the Collider2D component
    }

    private void Update()
    {
        // Handle the object dragging when it is picked up
        if (isPickedUp)
        {
            MoveObjectWithCursor();
        }

        // Check if the left mouse button is released to drop the object
        if (Input.GetButtonDown("Fire2")) // 1 is left mouse button (Fire2 is usually mapped to Left Click)
        {
            if (isPickedUp)
            {
                // Drop the object and return it to its original position
                DropObject();
            }
        }

        if (Input.GetButtonDown("Fire1")) // Fire1 (usually Left Click)
        {
            if (!isPickedUp)
            {
                // Try to pick up the object if it's not already picked up
                TryPickUpObject();
            }
        }
    }

    // Try to pick up the object when clicked
    private void TryPickUpObject()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Set z to 0 for 2D

        // Check if the mouse position is inside the object's collider
        if (objectCollider.OverlapPoint(mousePosition))
        {
            isPickedUp = true;

            // Calculate the offset between the mouse position and the object's position
            offset = transform.position - mousePosition;

            Debug.Log("Object picked up!");
        }
    }

    // Move the object with the cursor while it is picked up
    private void MoveObjectWithCursor()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Set z to 0 for 2D

        // Move the object based on the mouse position and the offset
        transform.position = mousePosition + offset;
    }

    // Drop the object and return it to its original position
    private void DropObject()
    {
        // Reset the flag and move the object back to its original position
        isPickedUp = false;
        transform.position = originalPosition;

        Debug.Log("Object dropped and returned to its original position.");
    }
}
