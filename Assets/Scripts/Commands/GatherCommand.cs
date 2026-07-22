using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Gather Action", menuName = "AI/Commands/Gather", order = 105)]
public class GatherCommand : ActionBase
{
    public override bool CanHandle(CommandContext cxt)
    {
        bool canHandle = cxt.Commandable is Worker 
                      && cxt.Hit.collider != null
                      && cxt.Hit.collider.TryGetComponent(out GatherableSupply _);

        return canHandle;
    }

    public override void Handle(CommandContext cxt)
    {
        Worker worker = (Worker)cxt.Commandable;
        GatherableSupply supply = cxt.Hit.collider.GetComponent<GatherableSupply>();
        
        worker.Gather(supply);
    }
}
}