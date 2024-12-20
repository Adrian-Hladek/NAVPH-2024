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
        timeUntilFire += Time.fixedDeltaTime;    

            if (timeUntilFire >= 1f / bps)
            {

                Shoot();
                timeUntilFire = 0f;
            }
        }

    }



    public void TowerSetup(int typeOfTower)
    {
        Debug.Log("Outside");

        // Load the prefab from the Resources folder
        GameObject loadedPrefab = Resources.Load<GameObject>("Prefabs/Small_Bullet");

        if (loadedPrefab != null)
        {
            bulletPrefab = loadedPrefab; // Correct way to set the property
            Debug.Log("Inside");
        }
        else
        {
            Debug.LogError("Bullet prefab could not be loaded from Resources.");
            Debug.Log(bulletPrefab);
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
