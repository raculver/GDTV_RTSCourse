using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Behahavior{

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Building Is In Progress", story: "[BaseBuilding] is being built.", category: "Conditions", id: "a43630ff4f51482feaaebb0b852e3f92")]
public partial class BuildingIsInProgressCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BaseBuilding> BaseBuilding;

    public override bool IsTrue()
    {
        return BaseBuilding.Value != null && BaseBuilding.Value.BuildStatus.State == BuildingProgress.BuildingState.Bulding;
    }
}

}