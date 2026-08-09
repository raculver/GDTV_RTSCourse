using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

namespace GameDevTV.RTS.Behahavior{
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BuildBuilding", story: "[Self] builds [BuildingType] at [TargetLocation] .", category: "Action/Units", id: "8753ac3a0a912470e08174d1f8821998")]
public partial class BuildBuildingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<BuildingSO> BuildingType;
    [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
    [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderContsruction;

    private float startBuildTime;
    private float invTotalBuildTime;
    private BaseBuilding completedBuilding;
    private Vector3 startingBuildingTransform;
    private Vector3 finishedBuildingTransform;

    protected override Status OnStart()
    {
        DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Build action start", DebugLogging.Instance.ACTION_BUILD_BUILDING);

        if (!HasValidInputs()){
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Build action has invalid inputs", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }
        startBuildTime = Time.time;
        invTotalBuildTime = 1.0f /BuildingType.Value.BuildTime;

        GameObject building = GameObject.Instantiate(BuildingType.Value.Prefab);
        
        if (!building.TryGetComponent(out completedBuilding) 
            || completedBuilding.MainRenderer == null){
            DebugLogging.Instance.Message("Build action has invalid inputs", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }
        BuildingUnderContsruction.Value = completedBuilding;
        Renderer ren = completedBuilding.MainRenderer;
        
        startingBuildingTransform = TargetLocation.Value - Vector3.up*ren.bounds.size.y;
        finishedBuildingTransform = TargetLocation.Value;
        completedBuilding.transform.position = startingBuildingTransform;

        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        float normalisedTime = (Time.time - startBuildTime) * invTotalBuildTime;
        completedBuilding.transform.position = Vector3.Lerp(startingBuildingTransform, finishedBuildingTransform, normalisedTime);
        return normalisedTime >= 1 ? Status.Success : Status.Running;
    }

    protected override void OnEnd(){
        if (CurrentStatus == Status.Success){
            completedBuilding.enabled = true;
            completedBuilding.SetNavMeshObstacleEnabled(true);
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Completed", DebugLogging.Instance.ACTION_BUILD_BUILDING);
        }
        else{
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Ended prematureley", DebugLogging.Instance.ACTION_BUILD_BUILDING);
        }
    }

    private bool HasValidInputs(){
        return Self.Value != null
            && BuildingType.Value != null
            && BuildingType.Value.Prefab != null;
    }
}

}