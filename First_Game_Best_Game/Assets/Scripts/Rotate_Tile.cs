using Unity;
using UnityEngine;
using System.Collections.Generic;


public class RotateTile : Action
{
    public RotateTile(int count, Action_Controller control)
    {
        type = ActionType.Rotate;
        target = null;

        actionCount = count;
        controller = control;
    }

    public override string GetActionTarget()
    {
        return Utils.tileTag;
    }

    public override LayerMask GetTargetLayers()
    {
        string[] masks = new string[]{Utils.tileLayer};
        return LayerMask.GetMask(masks);
    }

    public override bool IsExecutable(GameObject tile)
    {
        BoxCollider2D tileCollider = tile.GetComponent<BoxCollider2D>();
        if (tileCollider == null) 
        {
            Debug.LogError($"Tile {tile.name} has NO collider");
            return false;
        }

        return base.IsExecutable(tile);
    }

    public override void ExecuteAction()
    {
        base.ExecuteAction();

        // Perform rotation around center point
        BoxCollider2D tileCollider = target.GetComponent<BoxCollider2D>();
        target.transform.RotateAround(tileCollider.bounds.center, Vector3.forward, 90f);

        Cell_Update [] tileCells = target.GetComponentsInChildren<Cell_Update>();

        // Revert rotation of cells
        foreach (Cell_Update cell in tileCells) 
            cell.gameObject.transform.rotation = Quaternion.identity;

        Physics2D.SyncTransforms();

        // Update cells 
        foreach (Cell_Update cell in tileCells) cell.UpdateNearbyCells();
    }
}