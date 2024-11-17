using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartMapCreation : MonoBehaviour
{
    void Start()
    {
        // Find all objects with the tag "Bunka"
        GameObject[] bunkaObjects = GameObject.FindGameObjectsWithTag("Bunka");

        // Iterate through each object and call the Recalculate function
        foreach (GameObject bunka in bunkaObjects)
        {
            // Get the script component that contains the Recalculate function
            BunkaChange bunkaScript = bunka.GetComponent<BunkaChange>();

            // If the script exists on the object, call the Recalculate function
            if (bunkaScript != null)
            {
                bunkaScript.Recalculate();
            }
        }
    }
}
