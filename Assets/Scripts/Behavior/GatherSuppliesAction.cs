using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Gather Supplies", story: "[Agent] gathers [Amount] of supply from [GathSup] .", category: "Action/Units", id: "be088f7bb15216c033c1afcbb4b643ee")]
public partial class GatherSuppliesAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Amount;
    [SerializeReference] public BlackboardVariable<GatherableSupply> GathSup;
    [SerializeReference] public BlackboardVariable<SupplySO> TargetSupplySO;

    private float enterTime;
    Animator animator;

    protected override Status OnStart(){
        if (GathSup == null) return Status.Failure;
        if (Agent.Value.TryGetComponent<Animator>(out animator)){
            animator.SetBool(AnimationConstants.IS_GATHERING, true);
        }
        
        enterTime = Time.time;

        GathSup.Value.BeginGather();
        TargetSupplySO.Value = GathSup.Value.Supply;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GathSup.Value.Supply.BaseGatherTime + enterTime <= Time.time){
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd(){
        if (animator != null) animator.SetBool(AnimationConstants.IS_GATHERING, false);
        if (GathSup == null) return;

        if (CurrentStatus == Status.Success){
            
            Amount.Value = GathSup.Value.EndGather();
        }
        else{
            GathSup.Value.AbortGather();
        }
    }
}
}