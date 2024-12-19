using UnityEngine;


public class EmptyAction : Action
{
    public EmptyAction()
    {
        type = ActionType.None;

        actionCount = 0;
        controller = null;
    }

    public override string GetActionTarget()
    {
        return "";
    }

    public override LayerMask GetTargetLayers()
    {
        string[] masks = new string[]{Utils.tileLayer, Utils.cellLayer};
        return LayerMask.GetMask(masks);
    }

    public override bool IsExecutable(GameObject tile)
    {
        return true;
    }

    public override void ExecuteAction()
    {
        return;
    }
}