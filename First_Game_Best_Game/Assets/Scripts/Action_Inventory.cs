using UnityEngine;
using System.Collections.Generic;

public class Action_Inventory : MonoBehaviour
{
    [SerializeField] private ActionType [] inventoryActions;
    [SerializeField] private int [] inventoryUses;

    // Optional field (initlializes itself if null)
    [SerializeField] public Object_Holder actionHolder = null;

    private List<Action> actions = new List<Action>();

    void Awake()
    {
        if (actionHolder == null) actionHolder = FindObjectOfType<Object_Holder>();

        if (actionHolder == null)
        {
            Debug.LogError("Cound NOT find Object_Holder");
            return;
        }
        
        if (inventoryUses.Length != inventoryActions.Length)
        {
            Debug.LogError("WRONG number of actions and couns");
            return;
        }

        foreach (Action_Controller controller in FindObjectsOfType<Action_Controller>())
        {
            int actionCount = -1;

            for (int i = 0; i < inventoryActions.Length; i++) 
            {
                if (inventoryActions[i] == controller.actionType) actionCount = inventoryUses[i];
            }

            if (actionCount == -1) 
            {
                Debug.LogError($"Action with type {controller.actionType} does not have inventory.");
                continue;
            }

            Action act = new Action(controller.actionType, actionCount, controller);
            actions.Add(act);

            controller.selectedNewAction.AddListener(updateActionControllers);
            controller.selectedOldAction.AddListener(ClearPickedAction);
        }
    }

    void Start()
    {
        ClearPickedAction();
    }

    public void ClearPickedAction()
    {
        actionHolder.DropAction();
        updateActionControllers(ActionType.None);
    }
    
    public void updateActionControllers(ActionType selectedType)
    {
        foreach (Action action in actions) 
            action.selectAction(action.type == selectedType);


    }

    // Returns true if object holder should drop action, otherwise false
    public void TryPerformAction()
    {
        if (!actionHolder.HoldingAction())
        {
            Debug.LogError("Action type None cannot be performed");
            return;
        }

        // Find action in inventory - better use dicionary TODO
        Action performing_action = null; 
        foreach (Action action in actions)
        {
            if (action.type == actionHolder.actionValue) 
            {
                performing_action = action;
                break;
            }
        }

        if (performing_action == null)
        {
            Debug.LogError($"Cound not find action {actionHolder.actionValue} in inventory");
            ClearPickedAction();
            return;
        }

        string actionTag = performing_action.GetActionTarget();

        // Find object to perform action on
        GameObject target = null;
        RaycastHit2D[] possibleTargets = Utils.HitColliders(Action.GetActionLayers(performing_action.type));

        foreach(RaycastHit2D targ in possibleTargets)
        {
            if (targ.collider.CompareTag(actionTag))
            {
                target = targ.transform.gameObject;
                // Only one target 
                break;
            }
        }

        // Perform action
        if (target != null) 
        {
            performing_action.executeAction(target);

            // Clear action if it isnt executable
            if (!performing_action.isExecutable()) ClearPickedAction();
        }
    }

}
