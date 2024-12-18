using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class Map_Controller : MonoBehaviour
{
    Action_Inventory inventory = null;
    List <Highlight> activeHighLights = new List <Highlight>();

    void Awake()
    {
        inventory = FindObjectOfType<Action_Inventory>();

        if (inventory == null)
        {
            Debug.LogError("Cound NOT find Action_Inventory");
        }
    }

    void DeativateHightlights()
    {
        foreach (Highlight highlight in activeHighLights) highlight.Deactivate();

        activeHighLights.Clear();
    }

    void ActivateHightlights()
    {
        foreach (Highlight highlight in activeHighLights) highlight.Activate();
    }

    // Time.timeScale = 4;
    // TODO
    void Update()
    {
        DeativateHightlights();

        Action currentAction = inventory.GetCurrentAction();
        if (currentAction == null) 
        {
            Debug.LogError("");
            return;
        }
        else if (currentAction.type != ActionType.None)
        {   
            // Try Performing action
            if (Input.GetButtonDown("Fire1")) 
            {
                bool mouseOnMap = Utils.HitColliders(LayerMask.GetMask(Utils.mapLayer)).Length > 0;
                if (mouseOnMap) inventory.TryPerformAction();
            }

            // Drop action
            else if (Input.GetButtonDown("Fire2")) inventory.ClearPickedAction();
        }
        
        // Highlight stuff
        RaycastHit2D [] collisions = Utils.HitColliders(currentAction.GetTargetLayers());

        foreach (RaycastHit2D collision in collisions)
        {
            GameObject hit = collision.transform.gameObject;
            Highlight objectBorder = hit.GetComponent<Highlight>();

            if (objectBorder != null && currentAction.IsExecutable(hit)) activeHighLights.Add(objectBorder);
        }

        ActivateHightlights();
    }
}
