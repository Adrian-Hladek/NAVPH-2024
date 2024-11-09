using UnityEditor;
using UnityEngine;

public class ObjectUsageProtection : Editor
{
    // Add custom context menu to GameObject context menu
    [MenuItem("GameObject/Delete Object with Reference Check", true)]
    static bool ValidateDeleteObjectWithCheck()
    {
        // Ensure the context menu item is only valid if a GameObject is selected
        return Selection.activeGameObject != null;
    }

    [MenuItem("GameObject/Delete Object with Reference Check", false, 10)]
    static void DeleteObjectWithCheck()
    {
        // Get the currently selected GameObject
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogWarning("No object selected!");
            return;
        }

        bool isUsed = CheckIfObjectIsUsed(selectedObject);

        if (isUsed)
        {
            // If object is referenced elsewhere, show a confirmation dialog
            bool confirmed = EditorUtility.DisplayDialog(
                "Warning",
                $"{selectedObject.name} is being referenced by other objects. Are you sure you want to delete it?",
                "Yes, Delete",
                "Cancel"
            );

            if (confirmed)
            {
                // Proceed with deletion if confirmed
                Undo.DestroyObjectImmediate(selectedObject);
                Debug.Log($"{selectedObject.name} has been deleted.");
            }
            else
            {
                Debug.Log($"{selectedObject.name} deletion was canceled.");
            }
        }
        else
        {
            // If object is not referenced, delete immediately
            Undo.DestroyObjectImmediate(selectedObject);
            Debug.Log($"{selectedObject.name} has been deleted.");
        }
    }

    // Function to check if the object is used/referenced by other objects
    static bool CheckIfObjectIsUsed(GameObject obj)
    {
        bool isUsed = false;

        // Iterate through all GameObjects in the scene to check for references
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            // Skip checking the object itself
            if (go == obj) continue;

            // Check each component attached to the GameObject for references to the target object
            var components = go.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null) continue;

                var serializedObject = new SerializedObject(component);
                var serializedProperties = serializedObject.GetIterator();

                while (serializedProperties.Next(true))
                {
                    if (serializedProperties.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (serializedProperties.objectReferenceValue == obj)
                        {
                            // If object is referenced, return true
                            isUsed = true;
                            break;
                        }
                    }
                }
            }

            if (isUsed) break; // Exit early if a reference is found
        }

        return isUsed;
    }
}
