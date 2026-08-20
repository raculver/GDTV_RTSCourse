using System;
using System.Collections;
using GameDevTV.RTS.UI.Components;
using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.UI.Containers
{
    public class BuildingUnderConstructionUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private TextMeshProUGUI unitName;
        [SerializeField] private ProgressBar progressBar;
        
        public void EnableFor(BaseBuilding building)
        {
            gameObject.SetActive(true);
            unitName.SetText(building.unitSO.Name);
            StartCoroutine(AnimateBuildingProgressBarRoutine(building));
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator AnimateBuildingProgressBarRoutine(BaseBuilding building)
        {
            while (enabled && building.BuildStatus.Progress < 1)
            {
                if (building.BuildStatus.State != BuildingProgress.BuildingState.Bulding)
                {
                    yield return null;
                    continue;
                }

                float startTime = building.BuildStatus.StartTime;
                float stopTime = startTime + building.buildingSO.BuildTime;
                float fracComplent = (Time.time - startTime) / building.buildingSO.BuildTime; 
                progressBar.SetProgress(fracComplent);
                yield return null;
            }
        }
    }
}
