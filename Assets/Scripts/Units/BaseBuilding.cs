
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class BaseBuilding : AbstractCommandable
{
    private List<UnitSO> buildingQueue = new(MAX_SIZE_BUILD_QUEUE);
    private const int MAX_SIZE_BUILD_QUEUE = 7;
    private float timeBuildStart;
    private Coroutine buildRoutine;
    
    public float progress{get; private set;} = 0;
    public int QueueSize => buildingQueue.Count;
    public UnitSO [] Queue => buildingQueue.ToArray(); // give public array (copy) of the Queue

    public delegate void QueueUpdateEvent(UnitSO[] unitsInQueue);
    public event QueueUpdateEvent OnQueueUpdated;


    public void BuildUnit(UnitSO unit){
        if (buildingQueue.Count == MAX_SIZE_BUILD_QUEUE)
        {
            Debug.LogError("BuildUnit called after max capacity");
            return;
        }

        buildingQueue.Add(unit);
        if (buildingQueue.Count == 1){
            buildRoutine=StartCoroutine(DoBuildUnits());
        }
        else{
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }
        
    }

    private IEnumerator DoBuildUnits(){
        while (buildingQueue.Count > 0)
        {
            timeBuildStart = Time.time;
            UnitSO unit = buildingQueue[0];
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
            while (Time.time - timeBuildStart < unit.BuildTime){
                progress = Mathf.Clamp01((Time.time - timeBuildStart) / unit.BuildTime);
                yield return null;
            }
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            buildingQueue.RemoveAt(0);
        }   
        OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        progress = 0;
    }

    public void CancelBuildingUnit(int index){
        if (index < 0 || index >= buildingQueue.Count){
            Debug.LogError("Attemping to cancel a unit outside of build queue length.");
            return;
        }

        // if the index is zero, we cancel current item... need to stop coroutine
        if (index == 0)
        {
            buildingQueue.RemoveAt(index);
            StopAllCoroutines();
            if (buildingQueue.Count > 0){
                buildRoutine=StartCoroutine(DoBuildUnits());
            }
            else
            {
                OnQueueUpdated?.Invoke(buildingQueue.ToArray());        
            }
        }
        // if the index is higher, we just remove item from queue list
        else
        {
            buildingQueue.RemoveAt(index);
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }

    }
}
} 