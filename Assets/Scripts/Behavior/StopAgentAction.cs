using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Utilities;


namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopAgent", story: "[Agent] stops moving.", category: "Action/Navigation", id: "7ff43b49e5e45d7d4afd6205029b8bf5")]
public partial class StopAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private NavMeshAgent navMeshAgent;

    protected override Status OnStart(){
        if (Agent.Value.TryGetComponent<Animator>(out Animator animator)){
            animator.SetFloat(AnimationConstants.SPEED, 0f);
        }
        
        if (Agent.Value.TryGetComponent(out navMeshAgent)){
            navMeshAgent.ResetPath();
            return Status.Success;
        }

        return Status.Failure;
    }  
}
}