using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior{
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Destroy Ghost Building Action", story: "[GhostBuilding] is destroyed.", category: "Action/Units", id: "1221c3abfa87ce0206515e5f90f3aaa0")]
public partial class DestroyGhostBuildingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> GhostBuilding;

    protected override Status OnStart()
    {
        if (GhostBuilding.Value != null){
            GhostBuilding.Value.SetActive(false);
            GameObject.Destroy(GhostBuilding.Value);
        }
        return Status.Success;
    }
}

}