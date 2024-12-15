using UnityEngine;

// Utils class for global methods
public class Utils
{
    public const float epsilon = 0.001f;

    // Tags
    public const string mapTag = "Game Map";
    public const string tileTag = "Tile";
    public const string cellTag = "Cell";
    public const string pathTag = "Path";

    public const string actionTag = "Action";

    public const string buttonTag = "Button";
    public const string textTag = "Text";
    public const string imageTag = "Image";

    // Layers
    public const string mapLayer = "Game_Map";
    public const string tileLayer = "Tile_Layer";
    public const string cellLayer = "Cell_Layer";

    // Helper functions

    // Camera main = world camera
    // Gets all colliding objects
    public static RaycastHit2D[] HitColliders(LayerMask layers)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        return Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, layers);
    }

    // Sprites
    public static string getActionSprite(ActionType type)
    {
        if (type == ActionType.Rotate) return "Actions/Circle-Arrow";
        else if (type == ActionType.Create) return "Actions/Pickaxe";
        else if (type == ActionType.Delete) return "Actions/Pickaxe";

        Debug.LogError($"Wrong action type {type}");
        return "";
    }

    public const string spriteEmptyTile = "Canvas_7";

    // TODO - tower background clear ???
    // Update the sprite based on the boolean flags
    public static string getCellSprite(bool hasPath, bool hasTurret, bool canHavePath, bool hasRight, bool hasLeft, bool hasUpper, bool hasBottom)
    {
        // Zelen� mimo cesty 
        if (!hasPath && !hasTurret)
        {
            // Zelen� mimo cesty
            //Debug.Log("Green7");
            return "Path_options/Sprites/Canvas_7";
        }
        // zelena na ceste 
        else if (canHavePath && !hasTurret && !hasPath)
        {
            //Debug.Log("Green7");
            return "Path_options/Sprites/Canvas_7";
        }
        // stred "+" crossroadu
        else if (hasPath && hasUpper && hasBottom && hasLeft && hasRight)
        {
            //Debug.Log("Mid8");
            return "Path_options/Sprites/Canvas_8";
        }
        else if (hasPath && hasUpper && hasBottom)
        {
            if (hasRight)
            {
                // T ktor� ma top vlavo
                //Debug.Log("Tecko16");
                return "Path_options/Sprites/Canvas_16";
            }
            else if (hasLeft)
            {
                // T ktor� ma top vpravo
                //Debug.Log("Tecko13");
                return "Path_options/Sprites/Canvas_13";
            }
            else
            {
                //zhora dolu Vertical
                //Debug.Log("Straight11");
                return "Path_options/Sprites/Canvas_11";

            }
        }
        else if (hasPath && hasLeft && hasRight)
        {
            if (hasUpper)
            {
                // T ktor� ma top dolu
                //Debug.Log("Tecko17");
                return "Path_options/Sprites/Canvas_17";
            }
            else if (hasBottom)
            {
                // T ktor� ma top hore
                //Debug.Log("Tecko12");
                return "Path_options/Sprites/Canvas_12";
            }
            else
            {
                // pravo lavo horizontal
                //Debug.Log("Straight9");
                return "Path_options/Sprites/Canvas_9";

            }
        }
        else if (hasPath && hasUpper)
        {
            if (hasRight)
            {
                // L ktor� ma top a pravo
                //Debug.Log("Tecko15");
                return "Path_options/Sprites/Canvas_15";
            }
            else if (hasLeft)
            {
                // L ktor� ma top a lavo
                //Debug.Log("Tecko14");
                return "Path_options/Sprites/Canvas_14";
            }
            else
            {
                // dead end Top cesta ide zhora kon�i dolu
                //Debug.Log("Dead end1");
                return "Path_options/Sprites/Canvas_1";
            }
        }
        else if (hasPath && hasBottom)
        {
            if (hasRight)
            {
                // L ktor� ma Bot a pravo
                //Debug.Log("Tecko6");
                return "Path_options/Sprites/Canvas_6";
            }
            else if (hasLeft)
            {
                // L ktor� ma bot a lavo
                //Debug.Log("Tecko5");
                return "Path_options/Sprites/Canvas_5";
            }
            else
            {
                // dead end bottom cesta ide zdola kon�i hore
                //Debug.Log("Dead end4");
                return "Path_options/Sprites/Canvas_4";
            }
        }
        else if (hasPath && hasLeft)
        {
            // dead end Left cesta ide zlava kon�i pravo
            //Debug.Log("Dead end3");
            return "Path_options/Sprites/Canvas_3";
        }
        else if (hasPath && hasRight)
        {
            // dead end right cesta ide z prava kon�i vlavo
            //Debug.Log("Dead end2");
            return "Path_options/Sprites/Canvas_2";
        }
        else if (hasPath && !hasUpper && !hasBottom && !hasLeft && !hasRight)
        {
            // sam vojak v poli
            //Debug.Log("single10");
            return "Path_options/Sprites/Canvas_10";
        }
       
        Debug.LogError("Cell sprite NOT found");
        
        // TODO - placeholder
        return "";
    }

}

// TIPS

// Get every child of parent (even nested children)
//Transform [] children = parentGameObject.GetComponentsInChildren<Transform>();

// Find object/script of specific type
// T object = FindObjectOfType<T>();