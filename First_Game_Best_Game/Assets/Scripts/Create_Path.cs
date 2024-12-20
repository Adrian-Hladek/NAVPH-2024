using UnityEngine;


public class CreatePath : Action
{
    public CreatePath(int count, Action_Controller control)
    {
        type = ActionType.Create;
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
        string[] masks = new string[]{Utils.cellLayer};
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

        if (cellComponent.HasPath || !cellComponent.CanHavePath || cellComponent.HasTower) return false;

        return base.IsExecutable(cell);
    }

    public override void ExecuteAction()
    {
        base.ExecuteAction();

        Cell_Update cellComponent = target.GetComponent<Cell_Update>();

        //Physics2D.SyncTransforms();

        // Set the property value
        cellComponent.HasPath = true; 
        cellComponent.UpdateNearbyCells();
    }
}