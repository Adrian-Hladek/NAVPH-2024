using UnityEngine;

public class Object_Holder : MonoBehaviour
{
    [SerializeField] private Vector2 actionScale = Vector2.one;
    [SerializeField] private Vector2 cursorOffset = Vector2.zero;

    private ActionType currentAction;

    private SpriteRenderer actionImage = null;
    private Camera mainCamera = null; 

    public ActionType ActionType
    {
        get { return currentAction; }
    }

    void Awake()
    {
        // Set camera
        mainCamera = FindObjectOfType<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Could NOT find main camera for Action Holder");
        }

        // Set sprite
        actionImage = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if (actionImage == null)
        {
            Debug.LogError("Could NOT find sprite renderer for Action Holder");
        }
        else
        {
            actionImage.drawMode = SpriteDrawMode.Sliced;
            actionImage.size = actionScale;
        }

        // Set listeners
        foreach (Action_Controller controller in FindObjectsOfType<Action_Controller>())
            controller.selectedNewAction.AddListener(PickAction);

        DropAction();
    }

    private void MoveObjectWithCursor()
    {
        actionImage.size = actionScale;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        transform.position = mousePosition + new Vector3(cursorOffset.x, cursorOffset.y, 0);
    }

    void Update()
    {
        // Move the object with the cursor while it is picked up
        if (currentAction != ActionType.None) MoveObjectWithCursor();
    }

    public void PickAction(ActionType type)
    {
        Cursor.visible = false;
        currentAction = type;

        actionImage.enabled = true;
        actionImage.sprite = Resources.Load<Sprite>(Utils.getActionSprite(type));
    }

    public void DropAction()
    {
        Cursor.visible = true;

        currentAction = ActionType.None;

        actionImage.enabled = false;
    }
}