using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower_Update : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform towerRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Atribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float bps = 1f; //bullets per second
    [SerializeField] private int towerType;

    private float timeUntilFire;
    private Transform target;

    private const string CanonTransformPath = "RotatePoint/Canon";



    public float TargetingRange
    {
        get { return targetingRange; }
        set { targetingRange = value; }
    }


    public int TowerType
    {
        get { return towerType; }
        set { towerType = value; }

    }

    private void FixedUpdate()
    {
        timeUntilFire += Time.fixedDeltaTime;    

        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();
        

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            if (timeUntilFire >= 1f / bps)
            {
                Shoot();
                timeUntilFire = 0f;
            }
        }

    }

    public void ChangeSprite(string spritePath, string objectPath)
    {

        Transform targetTransform = transform.Find(objectPath);
        if (targetTransform == null)
        {
            Debug.LogError($"Object at path '{objectPath}' could not be found.");
            return;
        }

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = targetTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer component not found on {targetTransform.name}.");
            return;
        }

        // Load the new sprite dynamically from Resources
        Sprite newSprite = Resources.Load<Sprite>(spritePath);
        if (newSprite == null)
        {
            Debug.LogError($"Sprite at path '{spritePath}' could not be loaded.");
            return;
        }

        // Assign the new sprite to the SpriteRenderer
        spriteRenderer.sprite = newSprite;
        Debug.Log($"Sprite updated successfully for {targetTransform.name}!");
    }


    private void SetupBullet(string prefabPath)
    {
        // Load the bullet prefab from the Resources folder
        GameObject loadedPrefab = Resources.Load<GameObject>(prefabPath);

        if (loadedPrefab != null)
        {
            // Assign the loaded prefab to bulletPrefab
            bulletPrefab = loadedPrefab;
            Debug.Log($"Bullet prefab loaded successfully from '{prefabPath}'.");
        }
        else
        {
            // Log an error if the prefab could not be found
            Debug.LogError($"Bullet prefab could not be loaded from path '{prefabPath}'.");
        }
    }

    public void TowerSetup(int typeOfTower)
    {
        
        GameObject loadedPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        

        switch (typeOfTower)
        {
            case 0: // Example: Basic Tower

                targetingRange = 5f;
                //rotationSpeed = 200f;
                bps = 1f;
                Debug.Log("Basic Tower setup complete.");
                break;

            case 1: // Example: Advanced Tower

                SetupBullet("Prefabs/Small_Bullet");
                ChangeSprite("Towers/120blue", CanonTransformPath);

                targetingRange = 7f;
                //rotationSpeed = 200f;
                bps = 5f;
                Debug.Log("Fast Tower setup complete.");
                break;


            default: // Handle unknown tower types
                Debug.LogWarning("Unknown tower type specified.");



                break;
        }



    }


    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetTarget(target);
    }


    private void FindTarget()
    {

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }


    }

    private void RotateTowardsTarget()
    {
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        towerRotationPoint.rotation = Quaternion.RotateTowards(towerRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }


    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;

    }
    private void OnDrawGizmosSelected()
    {

        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);


    }




}
