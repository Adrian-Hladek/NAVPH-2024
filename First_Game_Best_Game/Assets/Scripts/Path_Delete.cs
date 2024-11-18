using UnityEngine;

public class PathDelete
{
    private void RemovePathToBunka(GameObject Bunka)
    {
        if (Bunka == null)
        {
            Debug.LogError("Bunka object is null. Aborting AddPathToBunka.");
            return;
        }

        Debug.LogWarning("Bunka clicked");
        Cell_Update bunkaChange = Bunka.GetComponent<Cell_Update>();
        if (bunkaChange != null)
        {
            Debug.Log($"Adding path to Bunka. Current HasPathValue: {bunkaChange.pathValue}");
            bunkaChange.pathValue = false; // Set the property value

            GameObject[] relatedObjects = bunkaChange.GetRelatedObjects(Bunka);

            if (relatedObjects == null || relatedObjects.Length == 0)
            {
                Debug.LogWarning("No related objects found.");
                return;
            }

            Debug.LogWarning($"Number of related objects: {relatedObjects.Length}");

            foreach (GameObject obj in relatedObjects)
            {
                Cell_Update objBunka = obj.GetComponent<Cell_Update>();
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