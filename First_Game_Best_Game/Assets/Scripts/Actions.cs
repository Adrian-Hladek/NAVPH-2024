using Unity;
using UnityEngine;
using System.Collections.Generic;

// When adding new actions, specify action 
public enum ActionType
{
    None = 0, 
    Rotate = 1,
    Create = 2, 
    Delete = 3,
    Turret_Basic = 4,
}

public class Action
{
    public ActionType type;
    public int actionCount;

    Action_Controller controller;

    public Action(ActionType actionType, int count, Action_Controller control)
    {
        type = actionType;
        actionCount = count;
        controller = control;
    }

    public bool IsExecutable()
    {
        return actionCount > 0;
    }

    void UpdateController()
    {
        if (IsExecutable()) controller.PickValue = true;
        else 
        {
            controller.PickValue = false;
            controller.SelectValue = false;
        }

        controller.UpdateCount(actionCount);
        controller.UpdateVisuals();
    }

    public void SelectAction(bool select)
    {
        controller.SelectValue = select;

        UpdateController();
    }

    public bool ExecuteAction(GameObject target)
    {
        if (!IsExecutable()) return false;

        bool success;

        switch (type)
        {
            case ActionType.Rotate:
                success = RotateTile(target);
                break;

            case ActionType.Create:
                success = AddPathToCell(target);
                break;

            case ActionType.Delete:
                success = RemovePathFromCell(target);
                break;

            case ActionType.Turret_Basic:
                success = AddBasicTower(target);
                break;

            default:
                success = false;
                break;
        }

        if (success) UseAction();
        return success;
    }
    
    void UseAction()
    {
        if (!IsExecutable())
        {
            Debug.LogError($"Action {type} with 0 count was executed");
            return;
        }

        actionCount -= 1;
        UpdateController();
    }

    public void AddAction (int countAdded)
    {
        actionCount += countAdded;
        UpdateController();
    }

    public string GetActionTarget()
    {
        string tag;

        switch (type)
        {
            case ActionType.Rotate:
                tag = Utils.tileTag;
                break;

            case ActionType.Create:
                tag = Utils.cellTag;
                break;

            case ActionType.Delete:
                tag = Utils.cellTag;
                break;

            case ActionType.Turret_Basic:
                tag = Utils.cellTag;
                break;

            default:
                tag = ""; 
                break;
        }

        return tag;
    }

    public static LayerMask GetActionLayers(ActionType type)
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



