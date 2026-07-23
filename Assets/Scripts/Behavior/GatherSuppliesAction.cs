using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Gather Supplies", story: "[Agent] gathers [Amount] of supply from [GathSup] .", category: "Action/Units", id: "be088f7bb15216c033c1afcbb4b643ee")]
public partial class GatherSuppliesAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> Amount;
    [SerializeReference] public BlackboardVariable<GatherableSupply> GathSup;
    
    private float enterTime;
    private bool isGathering = false;

    protected override Status OnStart()
    {
        if (GathSup.Value == null){
            DebugLogging.Instance.Message(
                $"{Agent.Name} Tried to gather a null object.",
                DebugLogging.Instance.ACTION_GATHER_SUP
            );
            return Status.Failure; // Expect the 
        }

        return Status.Running;
    }

    protected override Status OnUpdate(){
        if (GathSup.Value == null){
            return Status.Success; // Expected. GathSup will despawn when empty.
        }
        
        if (!isGathering && !GathSup.Value.IsBusy){
            // start gather
            enterTime = Time.time;
            GathSup.Value.BeginGather();
            isGathering = true;
        }

        if (isGathering && GathSup.Value.Supply.BaseGatherTime + enterTime < Time.time) {
            DebugLogging.Instance.Message(
                $"ACTION_GATHER_SUP: {Agent.Value.name} successfully gathered supplies {GathSup.Value.name}",
                DebugLogging.Instance.ACTION_GATHER_SUP
            );
            GathSup.Value.EndGather();
            isGathering = false;
            return Status.Success;
        }
        else
        {
            return Status.Running;
        }
    }

    protected override void OnEnd()
    {
    }
}

}