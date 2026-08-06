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

            buildCoroutine = StartCoroutine(UpdateUnitProgress());
            UpdateBuildQueueUI();
        }

        private void UpdateBuildQueueUI()
        {
            // RIFE.
            int i = 0;
            for (; i < building.QueueSize; i++)
            {
                int ldiifl = i; // locally defined index in for loop... meaning of inline function will elvolve with i otherwise.
                unitButtons[i].EnableFor(building.Queue[i], () => building.CancelBuildingUnit(ldiifl));
            }
            for (; i < unitButtons.Length; i++)
            {
                unitButtons[i].Disable();
            }

            if (building.QueueSize == 0){
                progressBar.SetProgress(0);
            }
        }

        public void Disable(){
            gameObject.SetActive(false); // Turn off UI
            if (building != null){
                building.OnQueueUpdated -= HandleQueueUpdated;    
            }
            buildCoroutine = null;
            building = null;
        }

        private IEnumerator UpdateUnitProgress() {
            while(building != null && building.QueueSize > 0){
                progressBar.SetProgress(building.progress);
                yield return null;
            }
            buildCoroutine = null;
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