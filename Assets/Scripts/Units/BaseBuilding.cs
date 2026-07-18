
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class BaseBuilding : AbstractCommandable
{
    private Queue<UnitSO> buildingQueue = new(MAX_SIZE_BUILD_QUEUE);
    private const int MAX_SIZE_BUILD_QUEUE = 7;
    private float timeBuildStart;
    
    public float progress{get; private set;} = 0;
    public int QueueSize => buildingQueue.Count;

    public delegate void QueueUpdateEvent(UnitSO[] unitsInQueue);
    public event QueueUpdateEvent OnQueueUpdated;

    public void BuildUnit(UnitSO unit){
        if (buildingQueue.Count == MAX_SIZE_BUILD_QUEUE)
        {
            Debug.LogError("BuildUnit called after max capacity");
            return;
        }

        buildingQueue.Enqueue(unit);
        if (buildingQueue.Count == 1){
            StartCoroutine(DoBuildUnits());
        }
        else{
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
        }
        
    }

    private IEnumerator DoBuildUnits(){
        while (buildingQueue.Count > 0)
        {
            timeBuildStart = Time.time;
            UnitSO unit = buildingQueue.Peek();
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
            while (Time.time - timeBuildStart < unit.BuildTime){
                progress = Mathf.Clamp01((Time.time - timeBuildStart) / unit.BuildTime);
                yield return null;
            }
//            yield return new WaitForSeconds(unit.BuildTime);
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            //OnQueue?.Invoke(buildingQueue.ToArray());
            buildingQueue.Dequeue();
        }   
        OnQueueUpdated?.Invoke(buildingQueue.ToArray());
    }
}

} 