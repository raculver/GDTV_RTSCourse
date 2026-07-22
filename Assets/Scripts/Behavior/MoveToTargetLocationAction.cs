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
        if (!Agent.Value.TryGetComponent(out navMeshAgent)){
            return Status.Failure;
        }

        if (Vector3.Distance(navMeshAgent.transform.position, TargetLocation.Value) <= navMeshAgent.stoppingDistance){
            return Status.Success;
        }
        
        navMeshAgent.SetDestination(TargetLocation);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){
            return Status.Success;
        }
        return Status.Running;
    }
}

}