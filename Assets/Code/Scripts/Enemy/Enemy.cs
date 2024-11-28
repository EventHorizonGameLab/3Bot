using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    GunSettings gun;
    [Title("Parameters")]
    [InfoBox("You can adjust theese values")]
    [SerializeField] float losRadius;
    [SerializeField] float rotationSpeed;
    [SerializeField] float stopDistanceToPlayer;
    [SerializeField] float patrolSpeed;
    [SerializeField] float chasingSpeed;
    [Title("PROGRAMMER ONLY")]
    [SerializeField] Transform muzzle;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask toIgnore;
    [SerializeField] List<Transform> waypoints;
    int wayPointIndex;
    bool playerEngaged;
    Collider[] colInRange;


    NavMeshAgent agent;
    Transform playerTransform;

    private void Awake()
    {
        gun = GetComponentInChildren<GunSettings>();
        agent = GetComponent<NavMeshAgent>();
        patrolSpeed = agent.speed;
        Debug.Log(waypoints.Count);
    }

    private void FixedUpdate()
    {
        
        if (PlayerIsInVision())
        {
            transform.LookAt(playerTransform.position);
            if (Vector3.Distance(playerTransform.position, transform.position) >= stopDistanceToPlayer)
            {
                agent.speed = chasingSpeed;
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
                playerEngaged = true;
            }
            else
            {
                agent.speed = patrolSpeed;
                agent.isStopped = true;
            }

            gun.Shoot();
        }
        else
        {
            playerEngaged = false;
            Patrol();
        }
    }


    bool PlayerIsInVision()
    {
        colInRange = Physics.OverlapSphere(transform.position, losRadius, playerLayer);
        Debug.LogWarning(colInRange.Length);
        if (colInRange.Length <= 0)
        {
            playerTransform = null;
            return false;
        }

        playerTransform = colInRange[0].transform;
        Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position + Vector3.up, playerTransform.position + Vector3.up);
        Debug.DrawRay(transform.position + Vector3.up, playerDirection * distance, Color.blue);
        if (Physics.Raycast(transform.position + Vector3.up, playerDirection, out RaycastHit hit, distance, ~toIgnore))
        {
            if (hit.collider != playerTransform) return false;
        }

      return true;
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed);
    }
        

    void Patrol()
    {
        Array.Clear(colInRange, 0, colInRange.Length);
        if (waypoints.Count < 1)
        {
            agent.speed = patrolSpeed;
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                wayPointIndex = (wayPointIndex + 1) % waypoints.Count;
                agent.SetDestination(waypoints[wayPointIndex].position);
            }
        }
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, losRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * 100);
    }
#endif
}
