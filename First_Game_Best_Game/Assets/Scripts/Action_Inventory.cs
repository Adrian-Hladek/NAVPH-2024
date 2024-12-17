using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Action_Inventory : MonoBehaviour
{
    [SerializeField] private ActionType [] inventoryActions;
    [SerializeField] private int [] inventoryUses;

    // Optional field (initlializes itself if null)
    [SerializeField] public Object_Holder actionHolder = null;
    [HideInInspector] public UnityEvent actionPerformed = new UnityEvent();

    List<Action> actions = new List<Action>();

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
            Debug.LogError("WRONG number of action types and uses");
            return;
        }

        // Add listeners to action controllers
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

            controller.selectedNewAction.AddListener(UpdateActionControllers);
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
        UpdateActionControllers(ActionType.None);
    }
    
    void UpdateActionControllers(ActionType selectedType)
    {
        foreach (Action action in actions) 
            action.SelectAction(action.type == selectedType);
    }

    public void TryPerformAction()
    {
        if (!actionHolder.HoldingAction())
        {
            Debug.LogError("Action type None cannot be performed");
            return;
        }

        // Find action in inventory - better use dictionary TODO
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
            bool success = performing_action.ExecuteAction(target);

            // Action was a success
            if (success)
            {
                // Clear action if it isnt executable
                if (!performing_action.IsExecutable()) ClearPickedAction();
                actionPerformed.Invoke();
            }
        }
    }

}
