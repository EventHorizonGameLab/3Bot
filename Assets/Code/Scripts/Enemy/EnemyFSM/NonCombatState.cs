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
        Debug.Log("sto cercando");
        if (controller.PlayerIsInRange() && controller.PlayerIsInVision())
        {
            
            controller.ChangeState(controller.combatState);
            return;
        }

        Patrol();

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
