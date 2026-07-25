using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Utilities;
using UnityEngine.InputSystem.Interactions;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to Target Location", story: "[Agent] moves to [TargetLocation] .", category: "Action/Navigation", id: "63841eb2b2910ef434b709bde778e1ed")]
public partial class MoveToTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    protected override Status OnStart()
    {
        Agent.Value.TryGetComponent<Animator>(out animator);

        DebugLogging.Instance.Message(
            $"ACTION_MOVE_TO_TARGET_POS: {Agent.Value.name} moving to {TargetLocation.Value}",
            DebugLogging.Instance.ACTION_MOVE_TO_TARGET_POS
        );

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
        if (animator != null){
            animator.SetFloat(AnimationConstants.SPEED, navMeshAgent.velocity.magnitude);
        }

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

    protected override void OnEnd(){
        if (animator != null){
            animator.SetFloat(AnimationConstants.SPEED, 0f);
        }
    }
}
}