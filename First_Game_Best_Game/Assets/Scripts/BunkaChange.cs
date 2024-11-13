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



    // na zistenie textury objektu potrebujem vedeiù vlastnosti z okolit˝ch objektov, tie s˙ uloûenÈ v tomto poli
    private List<GameObject> detectedObjects = new List<GameObject>();

    // Threshold value for comparing floating point numbers, prech·dzali veci ako 4<4 v ife pretoûe float bol prÌliû blizky ale nie rovnak˝ z nejakÈho dÙvodu to neölo pekne v Gride tak bolo treba nejakÈ overenie
    private const float epsilon = 0.01f;

    // dostaù Sprite/Obr·zok z objektu (Nwm preËo to pomenovali Sprite ale budiö)
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer component not found on the main object.");
        }


       
    }



    public void Recalculate()
    {
        // Reset flags to false at the start of each recalculation
        // AK by nebol reset robilo by to srandy :D na konci by bolo vöetko farebnÈ :D
        HasRight = false;
        HasLeft = false;
        HasUpper = false;
        HasBottom = false;
        detectedObjects.Clear(); // toto som myslel ûe bude robiù automaticky ale nono 

        // Sync transforms to ensure colliders are accurate after any changes
        Physics2D.SyncTransforms();

        // Log the main object
        Debug.Log($"Main object: {gameObject.name}");
        Debug.Log($"Global Position of {gameObject.name}: {transform.position}");

        // Find the colliders on the main object  (Bud˙ 4)
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

                   
                    //Debug.Log($"Detected object: {col.gameObject.name} at position: {position}");

                    // Check the `isPath` and `hasPath` properties vtedy vieme ûe je to polÌËko cesty a je tam cesta
                    
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

        
        if (detectedObjects.Count > 4)
        {
            // niekde m·me overlap, ktor˝ by nemal nastaù
            Debug.LogWarning($"More than 4 objects detected: {detectedObjects.Count} objects found.");
        }

        
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

        // Update the sprite based on the boolean flags

        if (!IsPath && !HasTurret)
        {
            // Zelen· mimo cesty
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
                // T ktorÈ ma top vlavo
                //Debug.Log("Tecko16");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_16");
            }
            else if (HasLeft)
            {
                // T ktorÈ ma top vpravo
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
                // T ktorÈ ma top dolu
                //Debug.Log("Tecko17");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_17");
            }
            else if (HasBottom)
            {
                // T ktorÈ ma top hore
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
                // L ktorÈ ma top a pravo
                //Debug.Log("Tecko15");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_15");
            }
            else if (HasLeft)
            {
                // L ktorÈ ma top a lavo
                //Debug.Log("Tecko14");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_14");
            }
            else
            {
                // dead end Top cesta ide zhora konËi dolu
                //Debug.Log("Dead end1");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_1");


            }
        }
        else if (HasPath && HasBottom)
        {


            if (HasRight)
            {
                // L ktorÈ ma Bot a pravo
                //Debug.Log("Tecko6");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_6");
            }
            else if (HasLeft)
            {
                // L ktorÈ ma bot a lavo
                //Debug.Log("Tecko5");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_5");
            }
            else
            {
                // dead end bottom cesta ide zdola konËi hore
                //Debug.Log("Dead end4");
                spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_4");

            }
        }
        else if (HasPath && HasLeft)
        {
            // dead end Left cesta ide zlava konËi pravo
            //Debug.Log("Dead end3");
            spriteRenderer.sprite = Resources.Load<Sprite>("Path_options/Sprites/Canvas_3");

        }
        else if (HasPath && HasRight)
        {
            // dead end right cesta ide z prava konËi vlavo
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
            Debug.LogWarning("Oh boy vsak cesta Ëo ma cesty nikde :D green");
            //spriteRenderer.sprite = defaultSprite;
        }

        Debug.Log("Sprite updated ");  // Log the updated sprite name
    }



}
