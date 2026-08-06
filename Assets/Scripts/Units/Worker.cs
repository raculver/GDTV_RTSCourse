using System;
using GameDevTV.RTS.Behahavior;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Utilities;
using Unity.Behavior;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class Worker : AbstractUnit, IBuildingBuilder
{
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
        GameObject instance = Instantiate(building.Prefab, targetLocation, Quaternion.identity);
        
        if (instance.TryGetComponent(out BaseBuilding baseBuilding)){
            baseBuilding.ShowGhostVisuals();
        }
        else
        {
            Debug.LogError($"Missing BaseBuilding on Prefab for BildingSO {building.name}");
        }

        // setup blackboard variables
        // ...
        // ...

        return instance;
    }
}

};
