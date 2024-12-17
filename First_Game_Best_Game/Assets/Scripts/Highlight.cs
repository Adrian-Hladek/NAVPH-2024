using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] SpriteRenderer border = null;

    private void Awake()
    {
        if (border == null) Debug.LogWarning($"Highlight object for {this.gameObject.name} is NOT set");
        Deactivate();
    }

    public void Activate()
    {
        if (border != null) border.enabled = true;
    }

    public void Deactivate()
    {
        if (border != null) border.enabled = false;
    }
}
