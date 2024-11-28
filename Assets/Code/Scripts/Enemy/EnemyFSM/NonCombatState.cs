using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NonCombatState : IEenemyState
{
    NavMeshAgent agent;
    EnemyController controller;
    bool playerDetected;
    public NonCombatState(EnemyController enemyController)
    { 
        controller = enemyController;
        agent = enemyController.GetComponent<NavMeshAgent>();
    }

    public void Enter()
    {
        playerDetected = false;
        agent.speed = controller.patrolSpeed;
    }

    public void Exit()
    {
        agent.ResetPath();
    }
        

    public void Process()
    {
        Collider[] colliders = Physics.OverlapSphere(controller.transform.position, controller.losRadius, controller.playerLayer);
        
        if (colliders.Length > 0)
        {
            controller.playerTransform = colliders[0].transform;
            controller.ChangeState(controller.combatState);
            return;
        }
        else Patrol();
        Debug.LogWarning(colliders.Length);
    }

    void Patrol()
    {
        
        if (controller.waypoints.Count < 1)
        {
            agent.speed = controller.patrolSpeed;
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                controller.wayPointIndex = (controller.wayPointIndex + 1) % controller.waypoints.Count;
                agent.SetDestination(controller.waypoints[controller.wayPointIndex].position);
            }
        }
    }
}
