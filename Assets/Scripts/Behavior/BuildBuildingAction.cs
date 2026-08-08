using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior{
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BuildBuilding", story: "[Self] builds [BuildingType] at [TargetLocation] .", category: "Action/Units", id: "8753ac3a0a912470e08174d1f8821998")]
public partial class BuildBuildingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<BuildingSO> BuildingType;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

    private float startBuildTime;
    private float invTotalBuildTime;
    private BaseBuilding completedBuilding;
    private Vector3 startingBuildingTransform;

    protected override Status OnStart()
    {
        if (!HasValidInputs()){
            DebugLogging.Instance.Message("Build action has invalid inputs", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }
        startBuildTime = Time.time;
        invTotalBuildTime = 1.0f /BuildingType.Value.BuildTime;

        GameObject building = GameObject.Instantiate(BuildingType.Value.Prefab);
        completedBuilding = building.GetComponent<BaseBuilding>();
        Renderer ren = completedBuilding.MainRenderer;
        
        startingBuildingTransform = TargetLocation.Value - Vector3.up*ren.bounds.size.y;
        completedBuilding.transform.position = startingBuildingTransform;

        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        float normalisedTime = (Time.time - startBuildTime) * invTotalBuildTime;
        completedBuilding.transform.position = Vector3.Lerp(startingBuildingTransform, TargetLocation.Value, normalisedTime);
        return normalisedTime >= 1 ? Status.Success : Status.Running;
    }

    protected override void OnEnd(){
        if (CurrentStatus == Status.Success){
            completedBuilding.enabled = true;
        }
    }


    private bool HasValidInputs(){
        return Self.Value != null
            && BuildingType.Value != null
            && BuildingType.Value.Prefab != null;
    }
}

}