using UnityEngine;


// When adding new actions, specify new action 
public enum ActionType
{
    None = 0, 
    Rotate = 1,
    Create = 2, 
    Delete = 3,
    Tower_Basic = 4,
    Tower_Move = 5,
    Tower_Fast = 6,
}

public abstract class Action
{
    public ActionType type;
    public int actionCount;

    protected Action_Controller controller;
    protected GameObject target;

    // Override methods
    public abstract string GetActionTarget();
    public abstract LayerMask GetTargetLayers();

    public virtual bool IsExecutable(GameObject newTarget)
    {
        if (!IsUsable()) return false;

        target = newTarget;
        return true;
    }

    public virtual void ExecuteAction()
    {
        actionCount -= 1;
        UpdateController();
    }

    public bool IsUsable()
    {
        return actionCount > 0;
    }

    public void SelectAction(bool select)
    {
        controller.IsSelected = select;

        if (!select) target = null;
        UpdateController();
    }

    public void AddAction (int countAdded)
    {
        actionCount += countAdded;
        UpdateController();
    }

    void UpdateController()
    {
        if (IsUsable()) controller.CanBePicked = true;
        else 
        {
            controller.CanBePicked = false;
            controller.IsSelected = false;
        }

        controller.UpdateCount(actionCount);
        controller.UpdateVisuals();
    }

    public static Action MapActionType(ActionType type, int count, Action_Controller controller)
    {
        switch (type)
        {
            case ActionType.Rotate:
                return new RotateTile(count, controller);

            case ActionType.Create:
                return new CreatePath(count, controller);

            case ActionType.Delete:
                return new DeletePath(count, controller);

            case ActionType.Tower_Basic:
                return new BuildTower(count, controller);

            case ActionType.Tower_Move:
                return new MoveTower(count, controller);

            case ActionType.Tower_Fast:
                return new BuildTowerFast(count, controller);
        }

        return null;
    }



}



