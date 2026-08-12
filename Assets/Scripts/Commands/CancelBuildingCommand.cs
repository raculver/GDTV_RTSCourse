using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Cancel Building", menuName = "Units/Commands/Cancel Building", order = 100)]
public class CancelBuildingCommand : ActionBase
{
    public override bool CanHandle(CommandContext cxt){
        return cxt.Commandable is IBuildingBuilder;
    }

    public override void Handle(CommandContext cxt){
        IBuildingBuilder buildingBuilder = cxt.Commandable as IBuildingBuilder;

        buildingBuilder.CancelBuilding();
    }
}
}