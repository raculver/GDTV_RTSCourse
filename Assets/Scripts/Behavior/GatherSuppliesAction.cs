#define DEBUG_MESSAGE_ACTION_GATHER_SUP
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

    protected override Status OnStart()
    {
        enterTime = Time.time;
        GathSup.Value.BeginGather();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
    if (GathSup.Value.Supply.BaseGatherTime + enterTime < Time.time)
        {
            Amount.Value = GathSup.Value.EndGather();
            #if DEBUG_MESSAGE_ACTION_GATHER_SUP
                Debug.Log($"DEBUG_MESSAGE_ACTION_GATHER_SUP: {Agent.Value.name} successfully gathered supplies {GathSup.Value.name}");
            #endif
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