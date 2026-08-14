using GameDevTV.RTS.Units;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : ActionBase{
        [field: SerializeField]  public BuildingSO BuildingType {get; private set;}

        public override bool CanHandle(CommandContext cxt){            
            if (cxt.Commandable is not IBuildingBuilder) return false;

            if (cxt.Hit.collider != null){
                return cxt.Hit.collider.TryGetComponent(out BaseBuilding building)
                    && BuildingType == building.buildingSO
                    && (building.BuildStatus.State == BuildingProgress.BuildingState.Paused
                        || building.BuildStatus.State == BuildingProgress.BuildingState.Destroyed
                );
            }
            
            return true;
        }

        public override void Handle(CommandContext cxt){
            IBuildingBuilder builder = (IBuildingBuilder)cxt.Commandable;
            if (cxt.Hit.collider != null && cxt.Hit.collider.TryGetComponent(out BaseBuilding building)){
                // this was a right click action
                builder.ResumeBuilding(building);
            }
            else{
                // this was an action to start building 
                builder.Build(BuildingType, cxt.Hit.point);
            }
        }
    }
} 