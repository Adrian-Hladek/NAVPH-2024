using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tower_Update : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform towerRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [Header("Atribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 200f;



    private Transform target;

    private void Update()
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
