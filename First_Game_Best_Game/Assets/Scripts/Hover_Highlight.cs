using UnityEngine;

public class Hover_Highlight : MonoBehaviour
{
    [SerializeField] private GameObject highlightChild;
    public GameObject requiredDeleteObject;
    public GameObject requiredCreateObject;
    [SerializeField] private string targetLayerName = "Bunka_Layer";

    private static int targetLayerMask; // Cache the layer mask for efficiency
    private bool isHovering = false;

    private void Awake()
    {
        if (highlightChild != null)
        {
            highlightChild.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Highlight child object is not assigned.");
        }

        // Cache the layer mask
        targetLayerMask = LayerMask.GetMask(targetLayerName);
    }

    private void Update()
    {
        // Perform a raycast to check for collisions
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, targetLayerMask);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {

           
            BunkaChange bunkaChange = hit.collider.gameObject.GetComponent<BunkaChange>();

            if (bunkaChange != null && bunkaChange.IsPathValue)
            {
                if (!isHovering && (ObjectPickup.heldObject == requiredDeleteObject || ObjectPickup.heldObject == requiredCreateObject || ObjectPickup.heldObject == null))
                {
                    isHovering = true;
                    if (highlightChild != null)
                    {
                        highlightChild.SetActive(true);
                    }
                    Debug.Log($"Mouse entered {gameObject.name}.");
                }
            }
            else if(bunkaChange != null)
            {
                if (!isHovering && ( ObjectPickup.heldObject == null))
                {
                    isHovering = true;
                    if (highlightChild != null)
                    {
                        highlightChild.SetActive(true);
                    }
                    Debug.Log($"Mouse entered {gameObject.name}.");
                }

            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                if (highlightChild != null)
                {
                    highlightChild.SetActive(false);
                }
                Debug.Log($"Mouse exited {gameObject.name}.");
            }
        }
    }
}
