using UnityEngine;
using System.Collections.Generic;

// When adding new actions, specify new action 
public enum ActionType
{
    None = 0, 
    Rotate = 1,
    Create = 2, 
    Delete = 3,
    Turret_Basic = 4,
}

public abstract class Action
{
    public ActionType type;
    public int actionCount;

    protected Action_Controller controller;

    // Override methods
    public abstract string GetActionTarget();
    public abstract bool IsExecutable(GameObject target);
    public virtual void ExecuteAction(GameObject target)
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
        controller.SelectValue = select;
        UpdateController();
    }

    public void AddAction (int countAdded)
    {
        actionCount += countAdded;
        UpdateController();
    }

    void UpdateController()
    {
        if (IsUsable()) controller.PickValue = true;
        else 
        {
            controller.PickValue = false;
            controller.SelectValue = false;
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
                return null;

            case ActionType.Delete:
                return null;

            case ActionType.Turret_Basic:
                return null;

            default:
                return null;
        }
    }

    public static LayerMask GetActionTargetLayers(ActionType type)
    {
        List <string> layerNames = new List <string>();

        switch (type)
        {
            case ActionType.Rotate:
                layerNames.Add(Utils.tileLayer);
                break;

            case ActionType.Create:
                layerNames.Add(Utils.cellLayer);
                break;

            case ActionType.Delete:
                layerNames.Add(Utils.cellLayer);
                break;

            case ActionType.Turret_Basic:
                layerNames.Add(Utils.cellLayer);
                break;

            case ActionType.None:
                layerNames.Add(Utils.tileLayer);
                layerNames.Add(Utils.cellLayer);
                break;
        }

        return LayerMask.GetMask(layerNames.ToArray());
    }

    bool AddPathToCell(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();

        // NO Cell_Update component
        if (cellComponent == null)
        {
            Debug.LogError($"Object {cell.name} does NOT have Cell_Update");
            return false;
        }

        // Cannot add path - exisitng path or path not possible
        if (cellComponent.pathValue || !cellComponent.possiblePath || cellComponent.turretValue) return false;

        Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.pathValue = true; 
        cellComponent.UpdateNearbyCells();

        return true;
    }

    bool RemovePathFromCell(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();

        // NO Cell_Update component
        if (cellComponent == null)
        {
            Debug.LogError($"Object {cell.name} does NOT have Cell_Update");
            return false;
        }

        // Cannot remove path - exisitng doenst exist or path not possible
        if (!cellComponent.pathValue || !cellComponent.possiblePath) return false;

        Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.pathValue = false; 
        cellComponent.UpdateNearbyCells();

        return true;
    }

    bool RotateTile(GameObject tile)
    {
        // Perform rotation around center point
        BoxCollider2D tileCollider = tile.GetComponent<BoxCollider2D>();

        if (tileCollider == null)
        {
            Debug.LogError($"Object {tile.name} has NO BoxCollider2D");
            return false;
        }

        tile.transform.RotateAround(tileCollider.bounds.center, Vector3.forward, 90f);

        Cell_Update [] tileCells = tile.GetComponentsInChildren<Cell_Update>();

        // Revert rotation of cells
        foreach (Cell_Update cell in tileCells) 
            cell.gameObject.transform.rotation = Quaternion.identity;


        Physics2D.SyncTransforms();

        // Update cells 
        foreach (Cell_Update cell in tileCells) 
            cell.UpdateNearbyCells();
        
        return true;
    }


    bool AddBasicTower(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();

        // NO Cell_Update component
        if (cellComponent == null)
        {
            Debug.LogError($"Object {cell.name} does NOT have Cell_Update");
            return false;
        }

        // Cannot add path - exisitng path or path not possible
        if (cellComponent.pathValue  || cellComponent.turretValue) return false;

        Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.turretValue = true;
        cellComponent.AddTurret("Turret", Resources.Load<Sprite>("Towers/Basic_Tower_transparent"),Vector2.zero,3);
            // AddTurret(string childName, Sprite sprite, Vector2 position, float radius)
        //cellComponent.UpdateNearbyCells();

        return true;
    }

}



