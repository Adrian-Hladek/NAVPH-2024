using System.Collections.Generic;
using UnityEngine;

public class BunkaChange : MonoBehaviour
{
    [SerializeField] private bool HasRight;
    [SerializeField] private bool HasLeft;
    [SerializeField] private bool HasUpper;
    [SerializeField] private bool HasBottom;

    [SerializeField] private bool IsPath;
    [SerializeField] private bool HasPath;
    [SerializeField] private bool HasTurret;

    private List<GameObject> detectedObjects = new List<GameObject>();
    private const float epsilon = 0.01f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer component not found on the main object.");
        }
    }

    public void Recalculate()
    {
        HasRight = false;
        HasLeft = false;
        HasUpper = false;
        HasBottom = false;
        detectedObjects.Clear();

        Physics2D.SyncTransforms();

        // Get all colliders in children, but filter out the ones directly on this GameObject
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        List<Collider2D> childColliders = new List<Collider2D>();

        foreach (Collider2D collider in allColliders)
        {
            if (collider.gameObject != gameObject)  // Only add colliders from child objects
            {
                childColliders.Add(collider);
            }
        }

        foreach (Collider2D mainCollider in childColliders)
        {
            Collider2D[] touchingObjects = new Collider2D[10];
            int overlapCount = Physics2D.OverlapCollider(mainCollider, new ContactFilter2D().NoFilter(), touchingObjects);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D col = touchingObjects[i];

                if (col.gameObject == gameObject)
                    continue;

                if (col.CompareTag("Bunka") && !detectedObjects.Contains(col.gameObject))
                {
                    string position = GetRelativePosition(mainCollider, col);
                    Debug.Log($"Detected object: {col.gameObject.name} at position: {position}");

                    BunkaChange bunkaProps = col.GetComponent<BunkaChange>();
                    if (bunkaProps != null && bunkaProps.IsPath && bunkaProps.HasPath)
                    {
                        switch (position)
                        {
                            case "Up":
                                HasUpper = true;
                                break;
                            case "Down":
                                HasBottom = true;
                                break;
                            case "Left":
                                HasLeft = true;
                                break;
                            case "Right":
                                HasRight = true;
                                break;
                        }
                    }

                    detectedObjects.Add(col.gameObject);
                }
            }
        }

        if (detectedObjects.Count > 4)
        {
            Debug.LogWarning($"More than 4 objects detected: {detectedObjects.Count} objects found.");
        }

        UpdateSpriteBasedOnFlags();
    }

    private string GetRelativePosition(Collider2D mainCollider, Collider2D detectedCollider)
    {
        Vector3 mainObjectPosition = transform.position;
        Vector3 detectedPosition = detectedCollider.transform.position;

        bool isYClose = Mathf.Abs(detectedPosition.y - mainObjectPosition.y) < epsilon;
        bool isXClose = Mathf.Abs(detectedPosition.x - mainObjectPosition.x) < epsilon;

        if (isYClose && isXClose)
        {
            return "Center";
        }

        if (detectedPosition.y > mainObjectPosition.y && !isYClose)
        {
            return "Up";
        }
        if (detectedPosition.y < mainObjectPosition.y && !isYClose)
        {
            return "Down";
        }
        if (detectedPosition.x > mainObjectPosition.x && !isXClose)
        {
            return "Right";
        }
        if (detectedPosition.x < mainObjectPosition.x && !isXClose)
        {
            return "Left";
        }

        return "Center";
    }

   


private void UpdateSpriteBasedOnFlags()
    {

        // Update the sprite based on the boolean flags

        if (!IsPath && !HasTurret)
        {
            // Zelená mimo cesty
            //Debug.Log("Green7");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_7");
        }
        else if (IsPath && !HasTurret && !HasPath)
        {
            // zelena na ceste 
            //Debug.Log("Green7");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_7");
        }
        else if (HasPath && HasUpper && HasBottom && HasLeft && HasRight)
        {
            // stred "+" crossroadu
            //Debug.Log("Mid8");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_8");
        }
        else if (HasPath && HasUpper && HasBottom)
        {



            if (HasRight)
            {
                // T ktoré ma top vlavo
                //Debug.Log("Tecko16");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_16");
            }
            else if (HasLeft)
            {
                // T ktoré ma top vpravo
                //Debug.Log("Tecko13");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_13");
            }
            else
            {
                //zhora dolu Vertical
                //Debug.Log("Straight11");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_11");

            }
        }
        else if (HasPath && HasLeft && HasRight)
        {


            if (HasUpper)
            {
                // T ktoré ma top dolu
                //Debug.Log("Tecko17");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_17");
            }
            else if (HasBottom)
            {
                // T ktoré ma top hore
                //Debug.Log("Tecko12");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_12");
            }
            else
            {
                // pravo lavo horizontal
                //Debug.Log("Straight9");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_9");

            }


        }
        else if (HasPath && HasUpper)
        {



            if (HasRight)
            {
                // L ktoré ma top a pravo
                //Debug.Log("Tecko15");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_15");
            }
            else if (HasLeft)
            {
                // L ktoré ma top a lavo
                //Debug.Log("Tecko14");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_14");
            }
            else
            {
                // dead end Top cesta ide zhora konèi dolu
                //Debug.Log("Dead end1");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_1");


            }
        }
        else if (HasPath && HasBottom)
        {


            if (HasRight)
            {
                // L ktoré ma Bot a pravo
                //Debug.Log("Tecko6");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_6");
            }
            else if (HasLeft)
            {
                // L ktoré ma bot a lavo
                //Debug.Log("Tecko5");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_5");
            }
            else
            {
                // dead end bottom cesta ide zdola konèi hore
                //Debug.Log("Dead end4");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_4");

            }
        }
        else if (HasPath && HasLeft)
        {
            // dead end Left cesta ide zlava konèi pravo
            //Debug.Log("Dead end3");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_3");

        }
        else if (HasPath && HasRight)
        {
            // dead end right cesta ide z prava konèi vlavo
            //Debug.Log("Dead end2");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_2");
        }
        else if (HasPath && !HasUpper && !HasBottom && !HasLeft && !HasRight)
        {
            // sam vojak v poli
            //Debug.Log("single10");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_10");
        }
        else
        {
            Debug.LogWarning("Oh boy vsak cesta èo ma cesty nikde :D green");
            //spriteRenderer.sprite = defaultSprite;
        }

        Debug.Log("Sprite updated ");  // Log the updated sprite name
    }



}
