using System;
using GameDevTV.RTS.Behahavior;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;
using Unity.Behavior;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units{

public class Worker : AbstractUnit, IBuildingBuilder
{
    [SerializeField] private ActionBase cancelBuildingCmd;

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
        
        if (tempGhostInstance.TryGetComponent(out BaseBuilding baseBuilding)){
            baseBuilding.ShowGhostVisuals();
        }
        else
        {
            Debug.LogError($"Missing BaseBuilding on Prefab for BildingSO {building.name}");
        }

        // setup blackboard variables
        graphAgent.SetVariableValue<Vector3>(BTVariables.BT_UNIT_TGT_POSITION, targetLocation);
        graphAgent.SetVariableValue<BuildingSO>(BTVariables.BT_UNIT_BUILDING_TYPE, building);
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_UNIT_BUILDING_GHOST, tempGhostInstance);
        graphAgent.SetVariableValue<UnitCommands>(BTVariables.BT_UNIT_COMMAND, UnitCommands.BuildBuilding);

        SetCommandsOverride(new ActionBase[] {cancelBuildingCmd});
        Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this));

        return tempGhostInstance;
    }

    public void CancelBuilding(){
        if ( graphAgent.GetVariable(BTVariables.BT_UNIT_BUILDING_GHOST, out BlackboardVariable<GameObject> ghostVariable)
             && ghostVariable.Value != null )
        {
            Destroy(ghostVariable.Value);
        }
        if ( graphAgent.GetVariable(BTVariables.BT_UNIT_BUILDING_CONSTR, out BlackboardVariable<BaseBuilding> buildingVariable)
             && buildingVariable.Value != null)
        {
            Destroy(buildingVariable.Value.gameObject);
        }
        SetCommandsOverride(Array.Empty<ActionBase>());
        //Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this));
        Stop();
    }
    }

};
