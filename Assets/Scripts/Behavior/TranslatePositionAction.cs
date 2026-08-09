using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

namespace GameDevTV.RTS.Behahavior{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at speed [Speed]", category: "Action/Navigation", id: "7b83459d6f0c95588b69c1360276c156")]
public partial class TranslatePositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<float> Speed;

    Vector3 startingPosition;
    float travelTime;
    float startTime;

    protected override Status OnStart()
    {
        DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Translating Position", DebugLogging.Instance.ACTION_BUILD_BUILDING);
        startingPosition = Self.Value.transform.position;
        float distance = (startingPosition - TargetLocation.Value).magnitude;
        travelTime = distance / Speed;
        startTime = Time.time;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float currentTime = Time.time;
        float fraction = (currentTime - startTime) / travelTime;
        if (fraction < 1){
            Self.Value.transform.position = Vector3.Lerp(startingPosition, TargetLocation, fraction);
            return Status.Running;
        }
        else {
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Translated position reached", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Success;
        }
        
    }

    protected override void OnEnd()
    {
        DebugLogging.Instance.Message($"ACTION_BUILD_BUILDING Translation ended with status {CurrentStatus}", DebugLogging.Instance.ACTION_BUILD_BUILDING);
    }
}

}