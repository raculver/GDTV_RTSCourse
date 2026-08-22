using System.Linq;
using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Build Building", menuName = "Units/Commands/Build Building")]
    public class BuildBuildingCommand : BaseCommand{
        [field: SerializeField]  public BuildingSO BuildingType {get; private set;}

        public override bool CanHandle(CommandContext cxt){
            // just what are you doing
            if (cxt.Commandable is not IBuildingBuilder) return false;

            // seems kind of weird to lock out a command if builder is already building, but ok
            if (cxt.Commandable is IBuildingBuilder builder && builder.IsBuildingNow) return false;

            // right click issued
            // resuming something we've already paid for
            if (cxt.MouseButtonUsed == UnityEngine.InputSystem.LowLevel.MouseButton.Right 
                && cxt.Hit.collider != null
                ){
                // bool resumeBuild = cxt.Hit.collider.TryGetComponent(out BaseBuilding building)
                //     //&& BuildingType == building.buildingSO
                //     && (building.BuildStatus.State == BuildingProgress.BuildingState.Paused
                //         || building.BuildStatus.State == BuildingProgress.BuildingState.Destroyed


                bool colliderIsBuilding = cxt.Hit.collider.TryGetComponent(out BaseBuilding yesItsABuilding);
                bool buildingStateIsAcceptable = cxt.Hit.collider.TryGetComponent(out BaseBuilding building)
                                                 && (building.BuildStatus.State == BuildingProgress.BuildingState.Paused
                                                    || building.BuildStatus.State == BuildingProgress.BuildingState.Destroyed

                );
                bool resumeBuild = buildingStateIsAcceptable;
                DebugLogging.Instance.Message($"ACTION_BUILD_BUILDING: Attemping resume build. Success Checks = {colliderIsBuilding}, {buildingStateIsAcceptable}.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
                if (colliderIsBuilding && !buildingStateIsAcceptable)
                {
                    DebugLogging.Instance.Message($"ACTION_BUILD_BUILDING: Unhappy with build state = {yesItsABuilding.BuildStatus.State}.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
                }
                DebugLogging.Instance.Message($"ACTION_BUILD_BUILDING: Attemping resume build. Success = {resumeBuild}.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
                return resumeBuild;
            }
            
            // left clicked
            // need to pay and check restrictions
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: New build", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            // we're placing a new building, and we're paying the supply cost. 
            return AllRestrictionsPass(cxt.Hit.point) && HasEnoughSupplies();
        }

        public override void Handle(CommandContext cxt){
            IBuildingBuilder builder = (IBuildingBuilder)cxt.Commandable;
            if (cxt.Hit.collider != null && cxt.Hit.collider.TryGetComponent(out BaseBuilding building)){
                // this was a right click action
                builder.ResumeBuilding(building);
                DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: Handling resume.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            }
            else if (AllRestrictionsPass(cxt.Hit.point) && HasEnoughSupplies()){
                // this was an action to start building 
                builder.Build(BuildingType, cxt.Hit.point);
                DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: Handling new build.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            }
        }

        public override bool IsLocked(CommandContext cxt) => !HasEnoughSupplies();

        private bool HasEnoughSupplies(){
            // return BuildingType.Cost.Minerals <= SuppliesController.amountMinerals
            //     && BuildingType.Cost.Gas <= SuppliesController.amountGas;
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: Supply check.", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            return SuppliesController.HasEnoughSupplies(BuildingType.Cost);
        }
    }
}