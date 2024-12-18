using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Action_Inventory : MonoBehaviour
{
    [SerializeField] private ActionType [] inventoryActions;
    [SerializeField] private int [] inventoryUses;

    [HideInInspector] public Object_Holder actionHolder = null;
    [HideInInspector] public UnityEvent actionPerformed = new UnityEvent();
    
    Dictionary<ActionType, Action> actions = new Dictionary<ActionType, Action>();
    Action emptyAction = new EmptyAction();

    void Awake()
    {
        actionHolder = FindObjectOfType<Object_Holder>();
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

            Action act = Action.MapActionType(controller.actionType, actionCount, controller);
            if (act != null)
            {
                actions[controller.actionType] = act;
                controller.selectedNewAction.AddListener(UpdateActionControllers);
                controller.selectedOldAction.AddListener(ClearPickedAction);
            }
            else Debug.LogError($"Action {controller.actionType} is NOT defined");
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
        foreach ((ActionType type, Action action) in actions) action.SelectAction(type == selectedType);
    }

    public Action GetCurrentAction()
    {
        Action holdingAction;

        if (actionHolder.ActionType == ActionType.None) return emptyAction;
        else if (!actions.TryGetValue(actionHolder.ActionType, out holdingAction)) return null;
        return holdingAction;
    }

    public void TryPerformAction()
    {
        // Find action in inventory
        Action performing_action = GetCurrentAction(); 
        
        if (performing_action == null)
        {
            Debug.LogError($"Action {actionHolder.ActionType} does NOT exist in inventory");
            ClearPickedAction();
            return;
        }
        // No action selected
        else if (performing_action.type == ActionType.None) return;

        // Find object to perform action on
        GameObject target = null;
        RaycastHit2D[] possibleTargets = Utils.HitColliders(performing_action.GetTargetLayers());

        foreach(RaycastHit2D targ in possibleTargets)
        {
            if (targ.collider.CompareTag(performing_action.GetActionTarget()))
            {
                target = targ.transform.gameObject;
                // Only one target 
                break;
            }
        }

        // Perform action
        if (target != null && performing_action.IsExecutable(target)) 
        {
            performing_action.ExecuteAction();

            // Clear action if it isnt executable
            if (!performing_action.IsUsable()) ClearPickedAction();
            actionPerformed.Invoke();
        }
    }

}
