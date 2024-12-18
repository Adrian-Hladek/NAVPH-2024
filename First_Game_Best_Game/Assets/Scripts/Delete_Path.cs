using Unity;
using UnityEngine;
using System.Collections.Generic;


public class DeletePath : Action
{
    public DeletePath(int count, Action_Controller control)
    {
        type = ActionType.Rotate;

        actionCount = count;
        controller = control;
    }

    public override string GetActionTarget()
    {
        return Utils.tileTag;
    }

    public override bool IsExecutable(GameObject tile)
    {
        if (!IsUsable()) return false;
        return true;
    }

    public override void ExecuteAction(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();

        // NO Cell_Update component
        if (cellComponent == null)
        {
            Debug.LogError($"Object {cell.name} does NOT have Cell_Update");
            return;
        }

        // Cannot remove path - exisitng doenst exist or path not possible
        if (!cellComponent.pathValue || !cellComponent.possiblePath) return;

        Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.pathValue = false; 
        cellComponent.UpdateNearbyCells();
    }
}