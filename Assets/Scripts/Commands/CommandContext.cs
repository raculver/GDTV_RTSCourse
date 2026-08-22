using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace GameDevTV.RTS.Commands
{
    public struct CommandContext
    {
        public AbstractCommandable Commandable {get; private set;}
        public RaycastHit Hit {get; private set;}
        public int UnitIndex {get; private set;}
        public MouseButton MouseButtonUsed {get; private set;}

        public CommandContext(AbstractCommandable commandable, RaycastHit hit, int unitIndex = 0, MouseButton mbu = MouseButton.Left){
            Commandable = commandable;
            Hit = hit;
            UnitIndex = unitIndex;
            MouseButtonUsed = mbu;
        }
    }
}