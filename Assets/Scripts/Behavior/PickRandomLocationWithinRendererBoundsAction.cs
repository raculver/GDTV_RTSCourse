using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PickRandomLocationWithinRendererBounds", story: "Set [TargetLocation] to a random point within [BuildingUnderConstruction] .", category: "Action/Units", id: "4a0ebc35a79f1fd906925808198834fd")]
public partial class PickRandomLocationWithinRendererBoundsAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

    protected override Status OnStart()
    {
        if (BuildingUnderConstruction.Value == null ||
            BuildingUnderConstruction.Value.MainRenderer == null)
        {
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: BuildingUnderConstruction is null", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }

        Renderer renderer = BuildingUnderConstruction.Value.MainRenderer;
        Bounds bounds = renderer.bounds;

        TargetLocation.Value = new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            TargetLocation.Value.y,
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );

        return Status.Success;
    }
}

}