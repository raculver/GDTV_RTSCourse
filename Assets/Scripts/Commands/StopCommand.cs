using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Stop Action", menuName = "Units/Commands/Stop", order = 101)]
public class StopCommand : ActionBase
{
    public override bool CanHandle(CommandContext cxt)
    {
        return cxt.Commandable is AbstractUnit;
    }

    public override void Handle(CommandContext cxt)
    {
        AbstractUnit unit = (AbstractUnit)cxt.Commandable;
        unit.Stop();
    }
}
}