#define DEBUG_MESSAGE_MOVES_TO_TARGET_POS
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Target Location", story: "[Agent] moves to [TargetLocation] .", category: "Action/Navigation", id: "63841eb2b2910ef434b709bde778e1ed")]
public partial class MoveToTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {       
        #if DEBUG_MESSAGE_MOVES_TO_TARGET_POS
            Debug.Log($"DEBUG_MESSAGE_MOVES_TO_TARGET_POS: {Agent.Value.name} moving to {TargetLocation.Value}");
        #endif

        if (!Agent.Value.TryGetComponent(out navMeshAgent)){
            return Status.Failure;
        }

        if (Vector3.Distance(navMeshAgent.transform.position, TargetLocation.Value) <= navMeshAgent.stoppingDistance){
            return StatusSuccess();
        }
        
        navMeshAgent.SetDestination(TargetLocation);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
            return StatusSuccess();
        }
        return Status.Running;
    }

    private Status StatusSuccess()
    {
        #if DEBUG_MESSAGE_MOVES_TO_TARGET_POS
            Debug.Log($"DEBUG_MESSAGE_MOVES_TO_TARGET_POS: {Agent.Value.name} arrived at {Agent.Value.transform.position}");
        #endif

        return Status.Success;
    }
}

}