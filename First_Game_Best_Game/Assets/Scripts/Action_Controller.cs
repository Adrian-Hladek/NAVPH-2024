using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Action_Controller : MonoBehaviour
{
    [SerializeField] public ActionType actionType;

    // UI 
    TMP_Text textField = null;
    SpriteRenderer actionImage = null;
    Button actionButton = null;

    // Events
    [HideInInspector] public UnityEvent<ActionType> selectedNewAction = new UnityEvent<ActionType>();
    [HideInInspector] public UnityEvent selectedOldAction = new UnityEvent();
    
    bool selected = false;
    bool pickable = true;

    public bool IsSelected
    {
        get { return selected; }
        set { selected = value; }
    }

    public bool CanBePicked
    {
        get { return pickable; }
        set { pickable = value; }
    }

    void Awake()
    {
        Transform [] children = gameObject.GetComponentsInChildren<Transform>();
        
        foreach(Transform child in children)
        {
            if (child.tag == Utils.textTag) textField = child.gameObject.GetComponentInChildren<TMP_Text>();
            if (child.tag == Utils.imageTag) actionImage = child.gameObject.GetComponentInChildren<SpriteRenderer>();
            if (child.tag == Utils.buttonTag) actionButton = child.gameObject.GetComponentInChildren<Button>();
        }

        if (actionImage == null) Debug.LogError($"Could not find image with tag {Utils.imageTag}");
        else actionImage.sprite = Resources.Load<Sprite>(Utils.getActionSprite(actionType));

        if (actionButton == null) Debug.LogError($"Could not find button with tag {Utils.buttonTag}");
        else actionButton.onClick.AddListener(ActionClicked);

        if (textField == null) Debug.LogError($"Could not find text with tag {Utils.textTag}");

        UpdateVisuals();
    }

    public void UpdateCount(int actionCount)
    {
        textField.text = actionCount.ToString();
        actionButton.interactable = pickable;
    }

    public void UpdateVisuals()
    {
        if (selected) actionImage.color = Color.black;
        else actionImage.color = Color.white;
    }

    void ActionClicked()
    {
        if (!pickable) return;

        if (selected) selectedOldAction.Invoke();
        else selectedNewAction.Invoke(actionType);
    }
}
