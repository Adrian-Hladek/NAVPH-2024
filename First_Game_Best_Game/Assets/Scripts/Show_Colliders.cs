using UnityEngine;

public class Show_Colliders : MonoBehaviour
{
    // This script will visualize all colliders attached to the object in Scene view
    private void OnDrawGizmos()
    {
        // Get the collider attached to this GameObject
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            // Set Gizmo color to visualize the collider
            Gizmos.color = Color.green;

            // Draw the appropriate Gizmo for the collider type
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = (BoxCollider2D)collider;
                Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
            }
            else if (collider is CircleCollider2D)
            {
                CircleCollider2D circleCollider = (CircleCollider2D)collider;
                Gizmos.DrawWireSphere(circleCollider.bounds.center, circleCollider.radius);
            }
            else if (collider is PolygonCollider2D)
            {
                PolygonCollider2D polygonCollider = (PolygonCollider2D)collider;

                // Loop through all points in the PolygonCollider2D
                Vector2[] points = polygonCollider.points;

                // Loop through the points to draw the polygon
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 start = transform.TransformPoint(points[i]);
                    Vector2 end = transform.TransformPoint(points[(i + 1) % points.Length]); // Wrap around to the first point

                    Gizmos.DrawLine(start, end); // Draw line between each point
                }
            }
            // Add more cases here if you use other types of colliders (e.g., EdgeCollider2D)
        }
    }
}
