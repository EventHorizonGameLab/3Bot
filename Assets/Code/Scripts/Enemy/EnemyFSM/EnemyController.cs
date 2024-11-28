using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    IEenemyState currentState;

    public NonCombatState nonCombatState;
    public CombatState combatState;
    //--\\

    [Title("Parameters")]
    [InfoBox("You can adjust theese values")]
    public float losRadius;
    public float rotationSpeed;
    public float stopDistanceToPlayer;
    public float patrolSpeed;
    public float chasingSpeed;
    [Title("PROGRAMMER ONLY")]
    public Transform muzzle;
    public LayerMask playerLayer;
    public LayerMask toIgnore;
    public List<Transform> waypoints;
    public Transform playerTransform;

    public int wayPointIndex;

    private void Start()
    {
        nonCombatState = new NonCombatState(this);
        combatState = new CombatState(this);
        ChangeState(nonCombatState);
    }
    public void ChangeState(IEenemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void FixedUpdate()
    {
        Debug.Log("IN RANGE" + PlayerIsInRange().ToString());
        Debug.Log("IN VISION" + PlayerIsInVision().ToString());
        currentState.Process();
        
    }

    public bool PlayerIsInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, losRadius, playerLayer);

        if (colliders.Length > 0)
        {
            playerTransform = colliders[0].transform;
            return true;
        }
        
        playerTransform = null;
        return false;
    }

    public bool PlayerIsInVision()
    {
        if (playerTransform == null)
        {
            Debug.Log("PlayerTransform is null.");
            return false;
        }

        Vector3 directionToPlayer = (playerTransform.position - transform.position);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        Debug.DrawRay(transform.position + Vector3.up, directionToPlayer, Color.blue);

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.collider.gameObject == playerTransform.gameObject)
            {
                return true;
            }
        }
                
        return false;
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
