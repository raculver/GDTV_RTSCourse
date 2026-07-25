using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using GameDevTV.RTS.Constants;
using System.Linq;
using GameDevTV.RTS.Utilities;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move to GatherableSupply", story: "[Agent] moves to [GathSup] or nearby not busy supply.", category: "Action/Navigation", id: "2813810296be152dcb533fde16925f0b")]
public partial class MoveToGatherableSupplyAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GatherableSupply> GathSup;
    [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7.0f);

    private NavMeshAgent navMeshAgent;
    private SupplySO targetSO;

    Vector3 targetLocation;
    Animator animator;

    protected override Status OnStart(){
        if (!ValidateAndSetupGatherableSupplyTarget()) return Status.Failure;
        if (!ValidateAndSetupNavAgent()) return Status.Failure;
        Agent.Value.TryGetComponent(out animator);
        return Status.Running;
    }

    private bool ValidateAndSetupGatherableSupplyTarget(){
        if (GathSup.Value != null){
            targetSO = GathSup.Value.Supply;
            return true;
        }
        else if (GathSup.Value == null && targetSO == null){
            // we don't have a gath sup and we also weren't able to grab the targetSO before it disappeared
            // give up
            return false;
        }
        else{
            // find a new target
            GatherableSupply newGathSup = ChooseClosestGatherableSupply();
            if (newGathSup == null){
                return false; // there are no alternative gathsups in the area
            }
            GathSup.Value = newGathSup; // we've found a new target
            return true;
        }
    }

    private bool ValidateAndSetupNavAgent(){
        if (!Agent.Value.TryGetComponent(out navMeshAgent)){
            DebugLogging.Instance.Message($"ACTION_MOVE_GATHER: {Agent.Value.name} has no nav agent.", DebugLogging.Instance.ACTION_MOVE_GATHER);
            return false;
        }
    
        targetLocation = GetGatherableSupplyLocation(GathSup.Value);
        DebugLogging.Instance.Message($"ACTION_MOVE_GATHER: {Agent.Value.name} setting target nav location to {targetLocation}.", DebugLogging.Instance.ACTION_MOVE_GATHER);
        navMeshAgent.SetDestination(targetLocation);
        return true;
    }

    private Vector3 GetGatherableSupplyLocation(GatherableSupply targetGathSup)
    {
//        if (targetGathSup == null) return targetLocation;

        return targetGathSup.TryGetComponent<Collider>(out Collider collider)
            ? collider.ClosestPoint(navMeshAgent.transform.position)
            : targetGathSup.transform.position;
    }

    protected override Status OnUpdate()
    {
        if (animator != null){
            animator.SetFloat(AnimationConstants.SPEED, navMeshAgent.velocity.magnitude);
        }

        if (navMeshAgent.pathPending || navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance)
            return Status.Running;  // keep navigating

        if (GathSup.Value != null && !GathSup.Value.IsBusy && GathSup.Value.AmountRemaining > 0)
            return Status.Success;  // arrived at resource

        GatherableSupply newGathSup = ChooseClosestGatherableSupply();
        if (newGathSup == null){
            DebugLogging.Instance.Message($"ACTION_MOVE_GATHER: {Agent.Value.name} found no free gatherables", DebugLogging.Instance.ACTION_MOVE_GATHER);
            DebugLogging.Instance.Message("ACTION_MOVE_GATHER: Warning. Deviation from CK's settings. This returns Status.Running.", DebugLogging.Instance.ACTION_MOVE_GATHER);
            return Status.Failure; // Return running... // CK uses Failure here and aborts the entire gather action (currently it will retrigger anyway, through the BT)
        }
        else{
            GathSup.Value = newGathSup;
            DebugLogging.Instance.Message($"ACTION_MOVE_GATHER: {Agent.Value.name} choosing to gather {GathSup.Value.name} instead.", DebugLogging.Instance.ACTION_MOVE_GATHER);
            targetLocation = GetGatherableSupplyLocation(GathSup.Value);
            navMeshAgent.SetDestination(targetLocation);  
            return Status.Running;
        }
    }

    private GatherableSupply ChooseClosestGatherableSupply()
    {
        DebugLogging.Instance.Message($"ACTION_MOVE_GATHER: {Agent.Value.name} chosing new closest gatherable supply",DebugLogging.Instance.ACTION_MOVE_GATHER);

        // Get all colliders that meet our requirements
        Collider[] nearbySupplies = Physics.OverlapSphere(
            Agent.Value.transform.position,
            SearchRadius,
            GameLayers.Supplies
        ).Where(col => col.TryGetComponent(out GatherableSupply sup)
                    && !sup.IsBusy
                    && sup.Supply.Equals(targetSO)
        ).ToArray();

        if (nearbySupplies.Length == 0){
            return null; // no gatherable supply found
        }

        ClosestColliderComparer comparer = new ClosestColliderComparer(Agent.Value.transform.position);
        Array.Sort(nearbySupplies, comparer);

        return nearbySupplies[0].GetComponent<GatherableSupply>();
    }

    protected override void OnEnd(){
        if (animator != null){
            animator.SetFloat(AnimationConstants.SPEED, 0f);
        }
    }

}
}