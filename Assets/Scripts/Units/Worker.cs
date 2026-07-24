using System;
using GameDevTV.RTS.Behahavior;
using GameDevTV.RTS.Environment;
using Unity.VisualScripting;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class Worker : AbstractUnit
{
    public void Gather(GatherableSupply supply)
    {
        graphAgent.SetVariableValue<GameObject>(BTVariables.BT_TARGET_GAME_OBJECT, supply.gameObject);
        graphAgent.SetVariableValue<GatherableSupply>(BTVariables.BT_UNIT_GATHERABLE_SUPPLY, supply);
        graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Gather);
    }
}

};
