using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] SpriteRenderer hoverImage = null;

    private void Awake()
    {
        if (hoverImage == null) Debug.LogWarning($"Highlight object for {this.gameObject.name} is NOT set");
        else Deactivate();
    }

    public void Activate()
    {
        if (hoverImage != null) hoverImage.enabled = true;
    }

    public void Deactivate()
    {
        if (hoverImage != null) hoverImage.enabled = false;
    }
}
