
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class BaseBuilding : AbstractCommandable
{
    private Queue<UnitSO> buildingQueue = new(MAX_SIZE_BUILD_QUEUE);
    private const int MAX_SIZE_BUILD_QUEUE = 7;

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
        
    }

    private IEnumerator DoBuildUnits(){
        while (buildingQueue.Count > 0)
        {
            UnitSO unit = buildingQueue.Peek();
            yield return new WaitForSeconds(unit.BuildTime);       
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            buildingQueue.Dequeue();
        }   
    }
}

} 