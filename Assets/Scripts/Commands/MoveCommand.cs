using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Move Action", menuName = "Units/Commands/Move", order = 100)]
public class MoveCommand : BaseCommand
{
    // ====== Weird Shared Reference Across ALL MoveCommand ====== 
    // Because BaseCommand inherits from ScriptableObject, it is an asset (a file on disk). 
    // When you put a MoveCommand into the AvailableCommands array on the prefab, every instantiated unit 
    // gets a reference to that same asset. They do not get their own separate copy.
    //
    // There is only ever one instance of MoveCommand.

    [SerializeField] private float fancyMoveRadius = 3.5f;

    private int unitsOnLayer = 0;
    private int maxUnitsOnLayer = 1;
    private float circleRadius = 0;
    private float angularOffset = 0;


    public override bool CanHandle(CommandContext cxt){
        return cxt.Commandable is AbstractUnit;
    }

    public override void Handle(CommandContext cxt){
        AbstractUnit unit = (AbstractUnit)cxt.Commandable;
        
        if (cxt.UnitIndex == 0){ ResetFancyMove();}

        Vector3 targetPosition = new Vector3(
            cxt.Hit.point.x + circleRadius * Mathf.Cos(angularOffset * unitsOnLayer),
            cxt.Hit.point.y,
            cxt.Hit.point.z + circleRadius * Mathf.Sin(angularOffset * unitsOnLayer)
        );

        unit.MoveTo(targetPosition);
        unitsOnLayer++;

        if (unitsOnLayer >= maxUnitsOnLayer)
        {
            unitsOnLayer = 0;
            circleRadius += unit.AgentRadius * fancyMoveRadius;
            maxUnitsOnLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.AgentRadius * 2));
            angularOffset = 2 * Mathf.PI / maxUnitsOnLayer;
        }
    }

    private void ResetFancyMove(){
        unitsOnLayer = 0;
        maxUnitsOnLayer = 1;
        circleRadius = 0;
        angularOffset = 0;
    }
}
}