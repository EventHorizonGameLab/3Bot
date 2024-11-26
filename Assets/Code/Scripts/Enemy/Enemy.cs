using Sirenix.OdinInspector;
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
            if (Vector3.Distance(playerTransform.position, transform.position) >= stopDistanceToPlayer)
            {
                agent.speed = chasingSpeed;
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
            else
            {
                agent.speed = patrolSpeed;
                agent.isStopped = true;
            }

            gun.Shoot();
        }
        else Patrol();
        
    }

    bool PlayerIsInVision()
    {
        Collider[] colInRange = (Physics.OverlapSphere(transform.position, losRadius, playerLayer));
        if (colInRange.Length <= 0) return false;
        Collider player = colInRange[0];
        playerTransform = player.gameObject.transform; 
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (Physics.Raycast(transform.position + Vector3.up, playerDirection, out RaycastHit hit, distance, ~toIgnore ))
        {
            if (hit.collider != player) return false;
        }

        if (RotateTowards(player.transform.position))
            return true;
        return false;
    }

    bool RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed);
        return Quaternion.Angle(transform.rotation, targetRot) < 5f;
    }

    void Patrol()
    {
        if (waypoints.Count < 1)
        {
            agent.speed = patrolSpeed;
            agent.isStopped = true;
        }
        else
        {
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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
        Gizmos.DrawRay(transform.position + Vector3.up , transform.forward * 100);
    }
#endif
}
