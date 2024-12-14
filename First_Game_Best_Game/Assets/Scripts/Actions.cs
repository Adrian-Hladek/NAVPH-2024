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
}

public class Action
{
    public ActionType type;
    public int actionCount;

    private Action_Controller controller;

    public Action(ActionType actionType, int count, Action_Controller control)
    {
        type = actionType;
        actionCount = count;
        controller = control;
    }

    public bool isExecutable()
    {
        return actionCount > 0;
    }

    private void updateController()
    {
        if (isExecutable()) controller.pickValue = true;
        else 
        {
            controller.pickValue = false;
            controller.selectValue = false;
        }

        controller.updateCount(actionCount);
        controller.updateVisuals();
    }

    public void selectAction(bool select)
    {
        controller.selectValue = select;

        updateController();
    }

    public bool executeAction(GameObject target)
    {
        if (!isExecutable()) return false;

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

            default:
                success = false;
                break;
        }

        if (success) useAction();
        return success;
    }
    
    private void useAction()
    {
        if (!isExecutable())
        {
            Debug.LogError($"Action {type} with 0 count was executed");
            return;
        }

        actionCount -= 1;
        updateController();
    }

    public void addAction (int countAdded)
    {
        actionCount += countAdded;
        updateController();
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

            case ActionType.None:
                layerNames.Add(Utils.tileLayer);
                layerNames.Add(Utils.cellLayer);
                break;
        }

        return LayerMask.GetMask(layerNames.ToArray());
    }


    private bool AddPathToCell(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();

        // NO Cell_Update component
        if (cellComponent == null)
        {
            Debug.LogError($"Object {cell.name} does NOT have Cell_Update");
            return false;
        }

        // Cannot add path - exisitng path or path not possible
        if (cellComponent.pathValue || !cellComponent.possiblePath) return false;

        Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.pathValue = true; 
        cellComponent.UpdateNearbyCells();

        return true;
    }

    private bool RemovePathFromCell(GameObject cell)
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

    private bool RotateTile(GameObject tile)
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
}



