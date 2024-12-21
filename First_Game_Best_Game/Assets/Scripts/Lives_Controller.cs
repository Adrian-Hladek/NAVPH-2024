using TMPro;
using UnityEngine;


public class Lives_Controller : MonoBehaviour
{
    TMP_Text textField = null;

    void Awake()
    {
        Transform [] children = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform child in children)
        {
            if (child.tag == Utils.textTag) textField = child.gameObject.GetComponentInChildren<TMP_Text>();
        }

        if (textField == null)
        {
            Debug.LogError($"Object {this.gameObject.name} could not FIND text with tag {Utils.textTag}");
            return;
        }

        Level_Manager level = FindObjectOfType<Level_Manager>();
        if (level == null)
        {
            Debug.LogError($"Object {this.gameObject.name} could not FIND Level_Manager");
            return;
        }

        level.changedLivesCount.AddListener(UpdateText);
    }

    void UpdateText(int currentLives)
    {
        if (currentLives < 0) currentLives = 0;
        textField.text = currentLives.ToString();
    }
}
