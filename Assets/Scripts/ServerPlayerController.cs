using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ServerPlayerController : MonoBehaviour
{
    NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public Vector3 UpdateNavTarget(Vector3 target)
    {
        agent.SetDestination(target);
        return target;
    }
}
