using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using GameDevTV.RTS.Units;
using System.Collections.Generic;
using GameDevTV.RTS.Constants;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Closest Command Post", story: "[Unit] finds nearest [CommandPost]", category: "Action/Units", id: "e147e9e83470a51ab68ce851ba9c4868")]
public partial class FindClosestCommandPostAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Unit;
    [SerializeReference] public BlackboardVariable<GameObject> CommandPost;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new(100);
    [SerializeReference] public BlackboardVariable<BuildingSO> CommandPostUnitSO;
    
    protected override Status OnStart()
    {
        Vector3 unitPosition = Unit.Value.transform.position;

        Collider[] colliders = Physics.OverlapSphere(unitPosition, SearchRadius.Value, GameLayers.Buildings);
     
        List<BaseBuilding> nearbyCommandPosts = new();
        foreach (Collider col in colliders){
            if (col.TryGetComponent(out BaseBuilding building)
                && building.unitSO.Equals(CommandPostUnitSO.Value)
                && building.BuildStatus.State == BuildingProgress.BuildingState.Completed)
            {
                nearbyCommandPosts.Add(building);
            }
        }

        if (nearbyCommandPosts.Count == 0){
            DebugLogging.Instance.Message(
                $"ACTION_FIND_CP: {Unit.Value.name} not find command post in radius {SearchRadius.Value} from {unitPosition}.\n"+
                $"ACTION_FIND_CP: Number of colliders found = {colliders.Length}",
                DebugLogging.Instance.ACTION_FIND_CP
            );           
            return Status.Failure;
        }

        nearbyCommandPosts.Sort(new ClosestCommandPostComparer(Unit.Value.transform.position));
        CommandPost.Value = nearbyCommandPosts[0].gameObject; 
        return Status.Success;
    }
}
}