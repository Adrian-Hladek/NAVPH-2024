using Unity;
using UnityEngine;
using System.Collections.Generic;


public class BuildTowerFast : Action
{
    public BuildTowerFast(int count, Action_Controller control)
    {
        type = ActionType.Tower_Basic;
        target = null;

        actionCount = count;
        controller = control;
    }

    public override string GetActionTarget()
    {
        return Utils.cellTag;
    }

    public override LayerMask GetTargetLayers()
    {
        string[] masks = new string[] { Utils.cellLayer };
        return LayerMask.GetMask(masks);
    }

    public override bool IsExecutable(GameObject cell)
    {
        Cell_Update cellComponent = cell.GetComponent<Cell_Update>();
        if (cellComponent == null)
        {
            Debug.LogError($"Cell {cell.name} has NO update");
            return false;
        }

        if (cellComponent.HasPath || cellComponent.HasTower) return false;

        return base.IsExecutable(cell);
    }

    public override void ExecuteAction()
    {

        base.ExecuteAction();

        Cell_Update cellComponent = target.GetComponent<Cell_Update>();

        //Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.HasTower = true;
        cellComponent.AddTower(1);

        // AddTurret(string childName, Sprite sprite, Vector2 position, float radius)
        //cellComponent.UpdateNearbyCells();
    }
}