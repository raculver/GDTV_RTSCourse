using GameDevTV.RTS.Units;
using GameDevTV.RTS.UI.Components;
using UnityEngine;
using System.Collections;

namespace GameDevTV.RTS.UI.Containers{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private UIBuildQueueButton[] unitButtons;
        [SerializeField] private ProgressBar progressBar;
        private BaseBuilding building;

        private Coroutine buildCoroutine;

        public void EnableFor(BaseBuilding item)
        {
            building = item;
            progressBar.SetProgress(building.progress);
            gameObject.SetActive(true); // Turn on UI
            building.OnQueueUpdated += HandleQueueUpdated;
            DebugLogging.Instance.Message($"Build Queue Subscribing to {building}.OnQueueUpdated", DebugLogging.Instance.BUILDING_BASEBUILDING);

            buildCoroutine = StartCoroutine(UpdateUnitProgress());
            UpdateBuildQueueUI();
        }

        private void UpdateBuildQueueUI()
        {
            if (building == null) return; // how did we get here?

            for (int i=0; i < building.QueueSize; i++)
            {
                // first buttons cancel the build
                int ldiifl = i; // locally defined index in for loop... meaning of inline function will elvolve with i otherwise.
                unitButtons[i].EnableFor(building.Queue[i], () => building.CancelBuildingUnit(ldiifl));
            }
            for (int i=building.QueueSize; i < unitButtons.Length; i++)
            {
                // remaining buttons are turned off
                unitButtons[i].Disable();
            }
        }

        public void Disable(){
            gameObject.SetActive(false); // Turn off UI
            if (building != null){
                building.OnQueueUpdated -= HandleQueueUpdated;    
                DebugLogging.Instance.Message($"Build Queue Unsubscribing to {building}.OnQueueUpdated", DebugLogging.Instance.BUILDING_BASEBUILDING);
            }
            buildCoroutine = null;
            building = null;
            DebugLogging.Instance.Message("Build UI disabled", DebugLogging.Instance.BUILDING_BASEBUILDING);
        }

        private IEnumerator UpdateUnitProgress() {
            while(building != null && building.QueueSize > 0){
                progressBar.SetProgress(building.progress);
                yield return null;
            }
            buildCoroutine = null;
            progressBar.SetProgress(0);
        }

        private void HandleQueueUpdated(AbstractUnitSO[] unitsInQueue){
            if (unitsInQueue.Length == 1 && buildCoroutine == null)
            {
                buildCoroutine = StartCoroutine(UpdateUnitProgress());
            }
            UpdateBuildQueueUI();
        }
    }
}