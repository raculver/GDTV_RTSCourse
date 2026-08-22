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
    [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderContsruction;

    private float startBuildTime;
    private float invTotalBuildTime;
    private float targetHealHealthCounter = 0f;
    private BaseBuilding completedBuilding;
    private Vector3 startingBuildingTransform;
    private Vector3 finishedBuildingTransform;
    private Renderer buildingRenderer;

    protected override Status OnStart()
    {
        DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Build action start", DebugLogging.Instance.ACTION_BUILD_BUILDING);

        if (!HasValidInputs()){
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING Build action has invalid inputs", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return Status.Failure;
        }

        if (BuildingUnderContsruction.Value == null){
            GameObject building = GameObject.Instantiate(BuildingType.Value.Prefab, TargetLocation, Quaternion.identity);
            if (!building.TryGetComponent(out completedBuilding) 
                || completedBuilding.MainRenderer == null)
            {
                DebugLogging.Instance.Message("Build action has invalid inputs", DebugLogging.Instance.ACTION_BUILD_BUILDING);
                return Status.Failure;
            }
        }
        else{
            completedBuilding = BuildingUnderContsruction.Value;
        }

        completedBuilding.StartBuilding(Self.Value.GetComponent<IBuildingBuilder>());
        startBuildTime = completedBuilding.BuildStatus.StartTime;
        
        invTotalBuildTime = 1.0f /BuildingType.Value.BuildTime;
        BuildingUnderContsruction.Value = completedBuilding;
        buildingRenderer = completedBuilding.MainRenderer;
        
        startingBuildingTransform = TargetLocation.Value - Vector3.up*buildingRenderer.bounds.size.y;
        finishedBuildingTransform = TargetLocation.Value;
        buildingRenderer.transform.position = startingBuildingTransform;

        return OnUpdate();
    }
    
    protected override Status OnUpdate()
    {
        float normalisedTime = (Time.time - startBuildTime) * invTotalBuildTime;
        buildingRenderer.transform.position = Vector3.Lerp(startingBuildingTransform, finishedBuildingTransform, normalisedTime);
        
        // ugh
        targetHealHealthCounter += Time.deltaTime*invTotalBuildTime*completedBuilding.MaximumHealth;
        if (targetHealHealthCounter > 1){
            int amountToHeal = Mathf.FloorToInt(targetHealHealthCounter);
            targetHealHealthCounter -= amountToHeal;
            completedBuilding.Heal(amountToHeal);
        }
        return normalisedTime >= 1 ? Status.Success : Status.Running;
    }

    protected override void OnEnd(){
        if (CurrentStatus == Status.Success){
            completedBuilding.enabled = true;
            //completedBuilding.SetNavMeshObstacleEnabled(true);?
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