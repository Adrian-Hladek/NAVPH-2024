using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Controller : MonoBehaviour
{
    private Action_Inventory inventory = null;
    private List <Highlight> activeHighLights = new List <Highlight>();

    void Awake()
    {
        inventory = FindObjectOfType<Action_Inventory>();

        if (inventory == null)
        {
            Debug.LogError("Cound NOT find Action_Inventory");
        }
    }

    private void DeativateHightlights()
    {
        foreach (Highlight highlight in activeHighLights) highlight.deactivate();

        activeHighLights.Clear();
    }

    private void ActivateHightlights()
    {
        foreach (Highlight highlight in activeHighLights) highlight.activate();
    }

    void Update()
    {
        DeativateHightlights();

        // Holding action - get input
        if (inventory.actionHolder.HoldingAction())
        {   
            bool mouseOnMap = Utils.HitColliders(LayerMask.GetMask(Utils.mapLayer)).Length > 0;

            // Try Performing action
            if (mouseOnMap && Input.GetButtonDown("Fire1")) 
            {
                inventory.TryPerformAction();
            }

            // Drop action
            else if (Input.GetButtonDown("Fire2")) 
            {
                Debug.Log($"Action {inventory.actionHolder.actionValue} droped");
                inventory.ClearPickedAction();
            }
        }

        // Highlight stuff
        RaycastHit2D [] collisions = Utils.HitColliders(Action.GetActionLayers(inventory.actionHolder.actionValue));

        foreach (RaycastHit2D collision in collisions)
        {
            GameObject hit = collision.transform.gameObject;
            Highlight objectBorder = hit.GetComponent<Highlight>();
            
            if (objectBorder != null) activeHighLights.Add(objectBorder);
        }

        ActivateHightlights();
    }
}
