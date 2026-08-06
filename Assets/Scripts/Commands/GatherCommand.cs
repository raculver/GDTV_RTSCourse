using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Gather Action", menuName = "Units/Commands/Gather", order = 105)]
public class GatherCommand : ActionBase
{
    [SerializeField] private AbstractUnitSO commandPostSO;

    public override bool CanHandle(CommandContext cxt){
        bool canHandle = cxt.Commandable is Worker
                        && cxt.Hit.collider != null
                        && ColliderIsGatherableOrCommandPost(cxt.Hit.collider);

        return canHandle;
    }

    public override void Handle(CommandContext cxt){
        Worker worker = (Worker)cxt.Commandable;
        if (cxt.Hit.collider.TryGetComponent<GatherableSupply>(out GatherableSupply supply)){
            // Gather supply
            worker.Gather(supply);
        }
        else if (ColliderIsCommandPost(cxt.Hit.collider) && worker.HasSupplies){
            // if player right clicks on a command post and has supplies, ReturnSupplies to CP
            worker.ReturnSupplies(cxt.Hit.collider.gameObject);
        }
        else{
            // right click was on command post but no supplies held.. just move to command post
            Vector3 commandPostPosition = cxt.Hit.collider.gameObject.transform.position;
            worker.MoveTo(commandPostPosition);
        }
    }

    private bool ColliderIsCommandPost(Collider collider) => collider.TryGetComponent(out BaseBuilding building) && building.unitSO.Equals(commandPostSO);
    private bool ColliderIsGatherableOrCommandPost(Collider collider) => collider.TryGetComponent(out GatherableSupply _) || ColliderIsCommandPost(collider);
}
}