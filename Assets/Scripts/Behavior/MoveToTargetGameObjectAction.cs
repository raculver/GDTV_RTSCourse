using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using UnityEditor;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to TargetGameObject", story: "[Agent] moves to [TargetGameObject] .", category: "Action/Navigation", id: "38ac85b633b115a29ed04c0a1c5d959e")]
public partial class MoveToTargetGameObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
    
    private NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {             

        if (!Agent.Value.TryGetComponent(out navMeshAgent)){
            return Status.Failure;
        }

        Vector3 targetLocation = TargetGameObject.Value.TryGetComponent<Collider>(out Collider collider)
                               ? collider.ClosestPoint(navMeshAgent.transform.position)
                               : TargetGameObject.Value.transform.position;

        DebugLogging.Instance.Message(
            $"ACTION_MOVE_TO_TARGET_POS: {Agent.Value.name} moving to {targetLocation}",
            DebugLogging.Instance.ACTION_MOVE_TO_TARGET_POS
        );


        if (Vector3.Distance(navMeshAgent.transform.position, targetLocation) <= navMeshAgent.stoppingDistance){
            return Status.Success;
        }
        
        navMeshAgent.SetDestination(targetLocation);
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
        DebugLogging.Instance.Message(
            $"MOVES_TO_TARGET_POS: {Agent.Value.name} arrived at {Agent.Value.transform.position}",
            DebugLogging.Instance.ACTION_MOVE_TO_TARGET_POS
        );

        return Status.Success;
    }


}

}