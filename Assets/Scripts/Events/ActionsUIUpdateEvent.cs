
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events{

    public struct ActionsUIUpdateEvent : IEvent
    {
        public AbstractCommandable Unit {get; private set;}  

        public ActionsUIUpdateEvent(AbstractCommandable unit)
        {
            Unit = unit;
        }
    }
}