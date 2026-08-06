using GameDevTV.RTS.Units;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : ActionBase{
        [field: SerializeField]  public BuildingSO BuildingType {get; private set;}

        public override bool CanHandle(CommandContext cxt)
        {
            return cxt.Commandable is IBuildingBuilder;
        }

        public override void Handle(CommandContext cxt)
        {
            IBuildingBuilder builder = (IBuildingBuilder)cxt.Commandable;
            builder.Build(BuildingType, cxt.Hit.point);
        }
    }
}