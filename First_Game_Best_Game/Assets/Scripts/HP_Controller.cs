using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Controller : MonoBehaviour
{
    Image healthbar = null;
    List <Image> components = new List <Image>();

    void Awake()
    {
        // Retrive HP bar image
        Image [] images = this.gameObject.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.gameObject.CompareTag(Utils.hpTag)) healthbar = img;
            else components.Add(img);
        }
        if (healthbar == null)
        {
            Debug.LogError($"Healthbar {this.gameObject.name} has NO image");
            return;
        }
        
        // Set listeners
        Enemy_Update enemy = this.gameObject.transform.parent.gameObject.GetComponentInChildren<Enemy_Update>();
        if (enemy == null)
        {
            Debug.LogError($"Healthbar {this.gameObject.name} has NO enemy");
            return;
        }
        enemy.despawn.AddListener(Deactivate);
        enemy.spawn.AddListener(Activate);
        enemy.hit.AddListener(UpdateBar);

        Deactivate(0);
    }

    void Deactivate(int livesTaken)
    {
        healthbar.enabled = false;
        foreach (Image img in components) img.enabled = false;
    }

    void Activate()
    {
        healthbar.enabled = true;
        foreach (Image img in components) img.enabled = true;
    }

    void UpdateBar(float hpPercent)
    {
        healthbar.fillAmount = hpPercent / 100;
    }
}
