using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Wave_Counter : MonoBehaviour
{
   TMP_Text textField = null;
   int totalNumberOfWaves = 0;
   int currentWaveNumber;

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

        currentWaveNumber = -1;
        level.updatedWaves.AddListener(UpdateText);
    }

    void UpdateText(Queue <Enemy_Spawner> waves)
    {

        if (totalNumberOfWaves == 0)
        {
            totalNumberOfWaves = waves.Count;
            currentWaveNumber = 0;
        }
        else currentWaveNumber += 1;

        if (currentWaveNumber > totalNumberOfWaves) textField.text = "DONE";
        else textField.text = $"{currentWaveNumber} / {totalNumberOfWaves}";
    }
}
