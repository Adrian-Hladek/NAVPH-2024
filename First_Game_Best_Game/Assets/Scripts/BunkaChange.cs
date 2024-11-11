using System.Collections.Generic;
using UnityEngine;

public class BunkaChange : MonoBehaviour
{
    // Store references to the detected objects
    private List<GameObject> detectedObjects = new List<GameObject>();

    // Method to recalculate and detect objects in contact with colliders of the main object
    public void Recalculate()
    {
        detectedObjects.Clear();  // Clear any previous results

        // Force the physics system to sync and update collider bounds (important after rotation)
        Physics2D.SyncTransforms();

        // Log the main object (the one calling this method)
        Debug.Log($"Main object: {gameObject.name}");


        // Print the global position of the main object
        Debug.Log($"Global Position of {gameObject.name}: {transform.position}");


       

        // Find the colliders on the main object
        Collider2D[] colliders = GetComponents<Collider2D>();

        // Iterate over each collider
        foreach (Collider2D mainCollider in colliders)
        {
            // Create an array to hold the results of the overlap check
            Collider2D[] touchingObjects = new Collider2D[10];  // Assuming maximum 10 objects to check

            // Use OverlapCollider to find objects that are overlapping with the collider
            int overlapCount = Physics2D.OverlapCollider(mainCollider, new ContactFilter2D().NoFilter(), touchingObjects);

            // Loop through each detected object
            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D col = touchingObjects[i];

                // Skip if the detected object is the same as the current object
                if (col.gameObject == gameObject)
                    continue;

                // Only consider objects with the "Bunka" tag
                if (col.CompareTag("Bunka") && !detectedObjects.Contains(col.gameObject))
                {
                    // Determine the relative position (Up, Down, Left, Right) of the detected object
                    string position = GetRelativePosition(mainCollider, col);

                    // Log which object was detected by which collider
                    Debug.Log($"Detected object: {col.gameObject.name} at position: {position}");

                    // Add the object to the list
                    detectedObjects.Add(col.gameObject);
                }
            }
        }

        // Check if more than 4 objects are detected
        if (detectedObjects.Count > 4)
        {
            Debug.LogWarning($"More than 4 objects detected: {detectedObjects.Count} objects found.");
        }


        // Optionally, log all detected objects
        Debug.Log("Detected objects: ");
        foreach (var obj in detectedObjects)
        {
            Debug.Log(obj.name);
        }
    }

    // Method to determine the relative position of the detected object using World Positions
    string GetRelativePosition(Collider2D mainCollider, Collider2D detectedCollider)
    {
        // Get the global position of the main object
        Vector3 mainObjectPosition = transform.position;

        // Get the global position of the detected object
        Vector3 detectedPosition = detectedCollider.transform.position;

        // Calculate the relative direction based on the detected object's position compared to the main object’s position
        if (detectedPosition.y > mainObjectPosition.y)
        {
            return "Up";  // Detected object is above the main object
        }
        if (detectedPosition.y < mainObjectPosition.y)
        {
            return "Down";  // Detected object is below the main object
        }
        if (detectedPosition.x > mainObjectPosition.x)
        {
            return "Right";  // Detected object is to the right of the main object
        }
        if (detectedPosition.x < mainObjectPosition.x)
        {
            return "Left";  // Detected object is to the left of the main object
        }

        // Fallback if the position matches exactly
        return "Center";
    }

}
