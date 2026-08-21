using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Rendering;
using static GameDevTV.RTS.Units.BaseBuilding;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingSelectedUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private SingleUnitSelectedUI singleUnitSelectedUI;
        [SerializeField] private BuildingBuildingUI buildingBuildingUI;
        [SerializeField] private BuildingUnderConstructionUI buildingUnderConstructionUI;

        private BaseBuilding selectedBuilding;

        public void EnableFor(BaseBuilding building)
        {
            selectedBuilding = building;
            UnsubscribeFromOnBuildingQueueUpdated();
            
            if (building.BuildStatus.State == BuildingProgress.BuildingState.Bulding)
            {
                Bus<BuildingSpawnEvent>.OnEvent += HandleBuildingSpawn;
            }
            
            selectedBuilding.OnQueueUpdated += HandleBuildingQueueUpdated;
            gameObject.SetActive(true);
            RefreshUI();
        }

        private void RefreshUI()
        {
            BaseBuilding building = selectedBuilding;

            if (building.BuildStatus.State == BuildingProgress.BuildingState.Completed)
            {
                buildingUnderConstructionUI.Disable();

                if (building.QueueSize == 0)
                {
                    singleUnitSelectedUI.EnableFor(building);
                    buildingBuildingUI.Disable();
                }
                else
                {
                    singleUnitSelectedUI.Disable();
                    buildingBuildingUI.EnableFor(building);
                }
            }
            else //(building.BuildStatus.State == BuildingProgress.BuildingState.Bulding)
            {
                buildingUnderConstructionUI.EnableFor(building);
                singleUnitSelectedUI.Disable();
                buildingBuildingUI.Disable();                
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            buildingBuildingUI.Disable();
            singleUnitSelectedUI.Disable();
            buildingUnderConstructionUI.Disable();

            UnsubscribeFromOnBuildingQueueUpdated();
            Bus<BuildingSpawnEvent>.OnEvent -= HandleBuildingSpawn;
            selectedBuilding = null;
        }

        private void UnsubscribeFromOnBuildingQueueUpdated()
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.OnQueueUpdated -= HandleBuildingQueueUpdated;
            }
        }

        private void HandleBuildingQueueUpdated(AbstractUnitSO[] _)
        {
            RefreshUI();
        }

        private void HandleBuildingSpawn(BuildingSpawnEvent buildingSpawnEvent)
        {
            if (buildingSpawnEvent.Building == selectedBuilding)
            {
                Bus<BuildingSpawnEvent>.OnEvent -= HandleBuildingSpawn;
                RefreshUI();
            }
        }
    }
}