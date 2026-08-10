using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine.AI;
using GameDevTV.RTS.Utilities;
using System.Data.Common;

namespace GameDevTV.RTS.Behahavior{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at speed [Speed]", category: "Action/Navigation", id: "7b83459d6f0c95588b69c1360276c156")]
public partial class TranslatePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    Vector3 startingPosition;
    float endTime;
    private Vector3 direction;
    private Transform selfTransform;

    protected override Status OnStart(){
        if (Self.Value == null) return Status.Failure;
        if (Self.Value.TryGetComponent(out navMeshAgent)){
            navMeshAgent.enabled = false;
        }
        Self.Value.TryGetComponent(out animator);
        
        selfTransform = Self.Value.transform;
        float distance = Vector3.Distance(selfTransform.position, TargetLocation.Value);
        direction = (TargetLocation.Value - selfTransform.position).normalized;
        endTime = Time.time + distance / Speed;

        // turn Annie Mater
        selfTransform.forward = direction;
        return Status.Running;
    }

    protected override Status OnUpdate(){
        if (Time.time > endTime) return Status.Success;

        if (animator != null) animator.SetFloat(AnimationConstants.SPEED, Speed);
        
        selfTransform.position += Speed * Time.deltaTime * direction;
        return Status.Running;
    }

    protected override void OnEnd(){
        if (navMeshAgent != null) navMeshAgent.enabled = true;
        if (animator != null) animator.SetFloat(AnimationConstants.SPEED, 0);
    }

}
}