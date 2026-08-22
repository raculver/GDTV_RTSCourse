using System;
using GameDevTV.RTS.Behahavior;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;
using Unity.Behavior;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace GameDevTV.RTS.Units{

public class Worker : AbstractUnit, IBuildingBuilder
{
    [SerializeField] private BaseCommand cancelBuildingCmd;
    public bool IsBuildingNow => graphAgent.GetVariable(BTVariables.BT_UNIT_COMMAND, out BlackboardVariable<UnitCommands> commandEnum)
                                   && commandEnum.Value == UnitCommands.BuildBuilding;

    public bool HasSupplies{
        get{
            if (graphAgent != null && graphAgent.GetVariable(BTVariables.BT_UNIT_GATHSUP_AMOUNT, out BlackboardVariable<int> amountHeld)){
                return amountHeld.Value > 0;
            }
            return false;
        }
    }

    protected override void Start(){
        base.Start();
        if (graphAgent.GetVariable(BTVariables.BT_UNIT_GATHSUP_EVT_CH, out BlackboardVariable<GatherSuppliesEventChannel> evtChannelVariable))
        {
            evtChannelVariable.Value.Event += HandleGatherSupplies;
        }
    }

    public void Gather(GatherableSupply supply)
    {
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_UNIT_TGT_GAME_OBJECT, supply.gameObject);
        graphAgent.SetVariableValue<GatherableSupply>(BTVariables.BT_UNIT_TGT_GATHSUP, supply);
        graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Gather);
    }   

    private void HandleGatherSupplies(GameObject agent, int amount, SupplySO gathSupSO){
        Bus<SupplyEvent>.Raise( new(amount, gathSupSO));
    }

    public void ReturnSupplies(GameObject targetCommandPost){
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_UNIT_TGT_CMD_POST, targetCommandPost);
        graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.ReturnSupplies);
    }

    public GameObject Build(BuildingSO building, Vector3 targetLocation)
    {
        GameObject tempGhostInstance = Instantiate(building.Prefab, targetLocation, Quaternion.identity);
        
        if (!tempGhostInstance.TryGetComponent(out BaseBuilding _)){
            Debug.LogError($"Missing BaseBuilding on Prefab for BildingSO {building.name}");
        }

        // setup blackboard variables
        graphAgent.SetVariableValue<Vector3>(BTVariables.BT_UNIT_TGT_POSITION, targetLocation);
        graphAgent.SetVariableValue<BuildingSO>(BTVariables.BT_UNIT_BUILDING_TYPE, building);
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_UNIT_BUILDING_GHOST, tempGhostInstance);
        graphAgent.SetVariableValue<UnitCommands>(BTVariables.BT_UNIT_COMMAND, UnitCommands.BuildBuilding);

        SetCommandsOverride(new BaseCommand[] {cancelBuildingCmd});
        Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this));
        
        PaySupplies(building.Cost);

        return tempGhostInstance;
    }

    public void ResumeBuilding(BaseBuilding building)
    {
        graphAgent.SetVariableValue<Vector3>(BTVariables.BT_UNIT_TGT_POSITION, building.transform.position);
        graphAgent.SetVariableValue(BTVariables.BT_UNIT_BUILDING_CONSTR, building);
        graphAgent.SetVariableValue<BuildingSO>(BTVariables.BT_UNIT_BUILDING_TYPE, building.buildingSO);
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_UNIT_BUILDING_GHOST, null);
        graphAgent.SetVariableValue<UnitCommands>(BTVariables.BT_UNIT_COMMAND, UnitCommands.BuildBuilding);
        
        SetCommandsOverride(new BaseCommand[] {cancelBuildingCmd});
        Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this));
    }

    public void CancelBuilding(){
        if ( graphAgent.GetVariable(BTVariables.BT_UNIT_BUILDING_GHOST, out BlackboardVariable<GameObject> ghostVariable)
             && ghostVariable.Value != null )
        {
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: Building cancellation part 1", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            Destroy(ghostVariable.Value);
        }
        if ( graphAgent.GetVariable(BTVariables.BT_UNIT_BUILDING_CONSTR, out BlackboardVariable<BaseBuilding> buildingVariable)
             && buildingVariable.Value != null)
        {
            DebugLogging.Instance.Message("ACTION_BUILD_BUILDING: Building cancellation part 2", DebugLogging.Instance.ACTION_BUILD_BUILDING);
            // RefundSupplies(buildingVariable.Value.buildingSO.Cost, 0.75f);
            Destroy(buildingVariable.Value.gameObject);
        }
        if (graphAgent.GetVariable(BTVariables.BT_UNIT_BUILDING_TYPE, out BlackboardVariable<BuildingSO> buildingType)){
            RefundSupplies(buildingType.Value.Cost, 0.75f);
        }
        
        SetCommandsOverride(Array.Empty<BaseCommand>());
        Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this));
        Stop();
    }
    }

};
