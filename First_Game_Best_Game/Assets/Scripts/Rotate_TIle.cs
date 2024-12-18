using Unity;
using UnityEngine;
using System.Collections.Generic;


public class RotateTile : Action
{
    public RotateTile(int count, Action_Controller control)
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

    public override void ExecuteAction(GameObject tile)
    {
        // Perform rotation around center point
        BoxCollider2D tileCollider = tile.GetComponent<BoxCollider2D>();

        if (tileCollider == null)
        {
            Debug.LogError($"Object {tile.name} has NO BoxCollider2D");
            return;
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
    }
}