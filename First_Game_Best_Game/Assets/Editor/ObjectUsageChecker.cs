using UnityEditor;
using UnityEngine;

public class ObjectUsageChecker : EditorWindow
{
    public GameObject targetObject;

    [MenuItem("Tools/Object Usage Checker")]
    public static void ShowWindow()
    {
        GetWindow<ObjectUsageChecker>("Object Usage Checker");
    }

    private void OnGUI()
    {
        // Allow the user to select a GameObject
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Check References"))
        {
            CheckObjectUsage(targetObject);
        }
    }

    private void CheckObjectUsage(GameObject obj)
    {
        if (obj == null)
        {
            Debug.Log("No object selected.");
            return;
        }

        bool isUsed = false;

        // Iterate through all GameObjects in the scene
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            // Skip the object itself
            if (go == obj) continue;

            // Check each component attached to the GameObject
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
                            Debug.Log($"Object {obj.name} is referenced by {go.name} (Component: {component.GetType()})");
                            isUsed = true;
                        }
                    }
                }
            }
        }

        if (!isUsed)
        {
            Debug.Log($"Object {obj.name} is not being used by any other objects.");
        }
        else
        {
            // If the object is being used, show a warning
            Debug.LogWarning($"{obj.name} is still being referenced by other objects!");
        }
    }

    // Add custom menu item to prevent deletion of GameObject
    [MenuItem("GameObject/Delete Object with Check", false, 10)]
    static void DeleteObjectWithCheck()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogWarning("No object selected!");
            return;
        }

        // Check if the object is being used in the scene
        bool isUsed = false;
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            if (go == selectedObject) continue;

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
                        if (serializedProperties.objectReferenceValue == selectedObject)
                        {
                            isUsed = true;
                            break;
                        }
                    }
                }
            }

            if (isUsed) break;
        }

        if (isUsed)
        {
            // Warn the user before allowing deletion
            bool confirmed = EditorUtility.DisplayDialog(
                "Warning",
                $"{selectedObject.name} is being referenced by other objects. Are you sure you want to delete it?",
                "Yes, Delete",
                "Cancel"
            );

            if (confirmed)
            {
                Undo.DestroyObjectImmediate(selectedObject); // Proceed with the deletion if confirmed
            }
        }
        else
        {
            // If not used, just delete the object
            Undo.DestroyObjectImmediate(selectedObject);
        }
    }
}
