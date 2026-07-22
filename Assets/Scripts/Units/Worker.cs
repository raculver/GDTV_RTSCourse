using System;
using GameDevTV.RTS.Behahavior;
using GameDevTV.RTS.Environment;
using UnityEngine;

namespace GameDevTV.RTS.Units{

public class Worker : AbstractUnit
{
    public void Gather(GatherableSupply supply)
    {
        graphAgent.SetVariableValue<Vector3>(BTVariables.BT_TARGET_POSITION, supply.transform.position);
        graphAgent.SetVariableValue<GatherableSupply>(BTVariables.BT_UNIT_GATHERABLE_SUPPLY, supply);
        graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Gather);
    }
}

};
