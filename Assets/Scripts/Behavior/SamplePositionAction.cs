using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime;

namespace GameDevTV.RTS.Behahavior{
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SamplePosition", story: "Set [TargetLocation] to the nearest point on the NavMesh to [Target] .", category: "Action/Navigation", id: "66965d60e1775a0a4410bef7e8df4f85")]
public partial class SamplePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Radius = new (5);



    protected override Status OnStart()
    {
        if (Target.Value == null 
            || !Target.Value.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            DebugLogging.Instance.Message($"ACTION_BUILD_BUILDING: issue with {Target.Value} in Sample position action.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }

        NavMeshQueryFilter queryFilter = new ();
        queryFilter.agentTypeID = navMeshAgent.agentTypeID;
        queryFilter.areaMask = navMeshAgent.areaMask;

        if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, Radius, queryFilter )){
            TargetLocation.Value = hit.position;
            return Status.Success;
        }
        else{
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: SamplePositionAction borked 2", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }
    }
}
}