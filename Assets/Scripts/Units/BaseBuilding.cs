using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units{

public class BaseBuilding : AbstractCommandable
{
    private List<AbstractUnitSO> buildingQueue = new(MAX_SIZE_BUILD_QUEUE);
    private const int MAX_SIZE_BUILD_QUEUE = 5; // 5 is hard coded into GUI
    private float timeBuildStart;
    private Coroutine buildRoutine;
    
    public float progress{get; private set;} = 0;
    public int QueueSize => buildingQueue.Count;
    public AbstractUnitSO [] Queue => buildingQueue.ToArray(); // give public array (copy) of the Queue

    [field: SerializeField] public MeshRenderer MainRenderer {get; private set;}
    [SerializeField] private Material primaryMaterial;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

    private BuildingSO buildingSO;

    public delegate void QueueUpdateEvent(AbstractUnitSO[] unitsInQueue);
    public event QueueUpdateEvent OnQueueUpdated;


    private void Awake(){
        buildingSO =  unitSO as BuildingSO;
    }

    protected override void Start(){
        base.Start();
        // if (navMeshObstacle != null) navMeshObstacle.enabled = true;
        if (MainRenderer != null) MainRenderer.material = primaryMaterial;
    }

    public void SetNavMeshObstacleEnabled(bool enabled){
        if (navMeshObstacle != null) navMeshObstacle.enabled = enabled;

    }

    public void BuildUnit(AbstractUnitSO unit){
        if (buildingQueue.Count == MAX_SIZE_BUILD_QUEUE){
            return;
            // Debug.LogError("BuildUnit called after max capacity");
            // return;
        }

        buildingQueue.Add(unit);
        DebugLogging.Instance.Message($"{this.name} adding {unit.name} to build queue.", DebugLogging.Instance.BUILDING_BASEBUILDING);
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
            AbstractUnitSO unit = buildingQueue[0];
            OnQueueUpdated?.Invoke(buildingQueue.ToArray());
            while (Time.time - timeBuildStart < unit.BuildTime){
                progress = Mathf.Clamp01((Time.time - timeBuildStart) / unit.BuildTime);
                yield return null;
            }
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            buildingQueue.RemoveAt(0);
            DebugLogging.Instance.Message($"{this.name} completed building {unit.name}.", DebugLogging.Instance.BUILDING_BASEBUILDING);
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

    public void ShowGhostVisuals(){
        MainRenderer.material = buildingSO.PlacementMaterial;
    }
}
} 