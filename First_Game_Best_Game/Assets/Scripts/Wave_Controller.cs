using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wave_Controller : MonoBehaviour
{
    [SerializeField] Color selectIconColor;
    [SerializeField] Sprite selectSprite;
    [SerializeField] Color disableColor;

    Sprite disselectSprite;
    Button waveButton;
    Image waveIcon;

    Level_Manager level;

   void Awake()
    {
        Transform [] children = this.gameObject.GetComponentsInChildren<Transform>();
    
        foreach(Transform child in children)
        {
            if (child.tag == Utils.buttonTag) waveButton = child.gameObject.GetComponentInChildren<Button>();
            if (child.tag == Utils.imageTag) waveIcon = child.gameObject.GetComponentInChildren<Image>();
        }

        level = FindObjectOfType<Level_Manager>();
        if (level == null)
        {
            Debug.LogError($"Object {this.gameObject.name} has NO Level_Manager");
            return;
        }

        disselectSprite = waveButton.image.sprite;
        waveButton.onClick.AddListener(StartNextWave);
        level.changedLevelState.AddListener(UpdateAccessibility);
    }

    void StartNextWave()
    {
        level.ActivateNextWave();
    }

    void UpdateAccessibility(LevelState state)
    {
        if (state == LevelState.Editing) 
        {
            waveButton.interactable = true;
            waveButton.image.sprite = disselectSprite;
            waveButton.image.color = Color.white;
            waveIcon.color = Color.white;
        }
        else if (state == LevelState.Fighting) 
        {
            waveButton.interactable = false;
            waveButton.image.sprite = selectSprite;
            waveButton.image.color = Color.white;
            waveIcon.color = selectIconColor;
        }
        else 
        {
            waveButton.interactable = false;
            waveButton.image.sprite = disselectSprite;
            waveButton.image.color = disableColor;
            waveIcon.color = Color.grey;
        }

    }
}
