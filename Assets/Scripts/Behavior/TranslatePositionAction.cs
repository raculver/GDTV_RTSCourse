using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at speed [Speed]", category: "Action/Navigation", id: "7b83459d6f0c95588b69c1360276c156")]
public partial class TranslatePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<float> Speed;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

}