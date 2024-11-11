using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BunkaChange : MonoBehaviour
{
    // Boolean flags for the object's neighboring paths, visible in the Inspector
    [SerializeField] private bool HasRight;
    [SerializeField] private bool HasLeft;
    [SerializeField] private bool HasUpper;
    [SerializeField] private bool HasBottom;

    [SerializeField] private bool IsPath;
    [SerializeField] private bool HasPath;
    [SerializeField] private bool HasTurret;

    





    // Store references to the detected objects
    private List<GameObject> detectedObjects = new List<GameObject>();

    // Threshold value for comparing floating point numbers
    private const float epsilon = 0.01f;

    // Method to recalculate and detect objects in contact with colliders of the main object
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the main object.");
        }


       
    }



    public void Recalculate()
    {
        // Reset flags to false at the start of each recalculation

        HasRight = false;
        HasLeft = false;
        HasUpper = false;
        HasBottom = false;
        detectedObjects.Clear();

        // Sync transforms to ensure colliders are accurate after any changes
        Physics2D.SyncTransforms();

        // Log the main object
        Debug.Log($"Main object: {gameObject.name}");
        Debug.Log($"Global Position of {gameObject.name}: {transform.position}");

        // Find the colliders on the main object
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D mainCollider in colliders)
        {
            // Use OverlapCollider to detect objects that are overlapping with this collider
            Collider2D[] touchingObjects = new Collider2D[10];
            int overlapCount = Physics2D.OverlapCollider(mainCollider, new ContactFilter2D().NoFilter(), touchingObjects);

            for (int i = 0; i < overlapCount; i++)
            {
                Collider2D col = touchingObjects[i];

                if (col.gameObject == gameObject)
                    continue;

                // Check if the detected object has the "Bunka" tag
                if (col.CompareTag("Bunka") && !detectedObjects.Contains(col.gameObject))
                {
                    string position = GetRelativePosition(mainCollider, col);

                    // Log which object was detected by which collider
                    Debug.Log($"Detected object: {col.gameObject.name} at position: {position}");

                    // Check the `isPath` and `hasPath` properties
                    BunkaChange bunkaProps = col.GetComponent<BunkaChange>();
                    if (bunkaProps != null && bunkaProps.IsPath && bunkaProps.HasPath)
                    {
                        // Set the appropriate flag based on position
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

        // Check if more than 4 objects are detected
        if (detectedObjects.Count > 4)
        {
            Debug.LogWarning($"More than 4 objects detected: {detectedObjects.Count} objects found.");
        }





        // here i want to do something with main object
        UpdateSpriteBasedOnFlags();

        
    }

    // Method to determine the relative position of the detected object using World Positions
    string GetRelativePosition(Collider2D mainCollider, Collider2D detectedCollider)
    {
        // Get the global position of the main object
        Vector3 mainObjectPosition = transform.position;

        // Get the global position of the detected object
        Vector3 detectedPosition = detectedCollider.transform.position;

        // Check if Y is close enough (within epsilon threshold)
        bool isYClose = Mathf.Abs(detectedPosition.y - mainObjectPosition.y) < epsilon;

        // Check if X is close enough (within epsilon threshold)
        bool isXClose = Mathf.Abs(detectedPosition.x - mainObjectPosition.x) < epsilon;

        // If both X and Y are close enough, consider the detected object as "Center"
        if (isYClose && isXClose)
        {
            return "Center";
        }

        // Calculate the relative direction based on the detected object's position
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

        return "Center";  // Fallback
    }


    private void UpdateSpriteBasedOnFlags()
    {

        Debug.LogWarning("Tu ma zmysel cita");
        // Check current sprite before changing
        //Sprite currentSprite = spriteRenderer.sprite;
        //Debug.Log("Current Sprite: " + currentSprite.name);  // Log the current sprite name

        // Update the sprite based on the boolean flags

        if (!IsPath && !HasTurret)
        {
            // Zelená mimo cesty
            Debug.Log("Green7");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_7");
        }
        else if (IsPath && !HasTurret && !HasPath)
        {
            // zelena na ceste 
            Debug.Log("Green7");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_7");
        }
        else if (HasPath && HasUpper && HasBottom && HasLeft && HasRight)
        {
            Debug.Log("Mid8");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_8");
        }
        else if (HasPath && HasUpper && HasBottom)
        {



            if (HasRight)
            {
                // T ktoré ma top vlavo
                Debug.Log("Tecko16");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_16");
            }
            else if (HasLeft)
            {
                // T ktoré ma top vpravo
                Debug.Log("Tecko13");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_13");
            }
            else
            {
                //zhora dolu Vertical
                Debug.Log("Straight11");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_11");

            }
        }
        else if (HasPath && HasLeft && HasRight)
        {


            if (HasUpper)
            {
                // T ktoré ma top dolu
                Debug.Log("Tecko17");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_17");
            }
            else if (HasBottom)
            {
                // T ktoré ma top hore
                Debug.Log("Tecko12");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_12");
            }
            else
            {
                // pravo lavo horizontal
                Debug.Log("Straight9");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_9");

            }


        }
        else if (HasPath && HasUpper)
        {



            if (HasRight)
            {
                // L ktoré ma top a pravo
                Debug.Log("Tecko15");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_15");
            }
            else if (HasLeft)
            {
                // L ktoré ma top a lavo
                Debug.Log("Tecko14");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_14");
            }
            else
            {
                // dead end Top cesta ide zhora konèi dolu
                Debug.Log("Dead end1");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_1");


            }
        }
        else if (HasPath && HasBottom)
        {


            if (HasRight)
            {
                // L ktoré ma Bot a pravo
                Debug.Log("Tecko6");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_6");
            }
            else if (HasLeft)
            {
                // L ktoré ma bot a lavo
                Debug.Log("Tecko5");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_5");
            }
            else
            {
                // dead end bottom cesta ide zdola konèi hore
                Debug.Log("Dead end4");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_4");

            }
        }
        else if (HasPath && HasLeft)
        {
            // dead end Left cesta ide zlava konèi pravo
            Debug.Log("Dead end3");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_3");

        }
        else if (HasPath && HasRight)
        {
            // dead end right cesta ide z prava konèi vlavo
            Debug.Log("Dead end2");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_2");
        }
        else if (HasPath && !HasUpper && !HasBottom && !HasLeft && !HasRight)
        {
            // sam vojak v poli
            Debug.Log("single10");
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
