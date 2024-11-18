using System.Data;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] SpriteRenderer border = null;

    private void Awake()
    {
        if (border == null)
        {
            Debug.LogWarning($"Highlight object for {this.gameObject.name} is NOT set");
        }
        
        deactivate();
    }

    public void activate()
    {
        if (border != null) border.enabled = true;
    }

    public void deactivate()
    {
         if (border != null) border.enabled = false;
    }
}
