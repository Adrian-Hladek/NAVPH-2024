using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game_Speed_Controller : MonoBehaviour
{
    [SerializeField] float gameSpeed = -1f;
    [SerializeField] Color selectImageColor;

    Button speedButton;
    Image speedImage;

    Level_Manager level;

    void Awake()
    {
        Transform [] children = gameObject.GetComponentsInChildren<Transform>();
    
        foreach(Transform child in children)
        {
            if (child.tag == Utils.buttonTag) speedButton = child.gameObject.GetComponentInChildren<Button>();
            if (child.tag == Utils.imageTag) speedImage = child.gameObject.GetComponentInChildren<Image>();
        }

        if (gameSpeed < 0 || gameSpeed > 10)
        {
            Debug.LogError($"Button {this.gameObject.name} has wrong speed settings");
            return;
        }

        level = FindObjectOfType<Level_Manager>();
        if (level == null)
        {
            Debug.LogError($"Button {this.gameObject.name} has NO Level_Manager");
            return;
        }

        speedButton.onClick.AddListener(Selected);
        level.changedLevelState.AddListener(UpdateAccessibility);

        speedButton.interactable = false;
        Disselected();
    }

    void Selected()
    {
        level.SetLevelSpeed(gameSpeed);
        speedImage.color = selectImageColor;
    }

    public void Disselected()
    {
        speedImage.color = Color.white;
    }

    void UpdateAccessibility(LevelState state)
    {
        if (state == LevelState.GameOver || state == LevelState.Finished) 
        {
            speedButton.interactable = false;
            speedImage.color = Color.black;
        }
        else speedButton.interactable = true;
    }

}
