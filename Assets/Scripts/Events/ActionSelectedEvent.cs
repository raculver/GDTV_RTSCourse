using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;

namespace GameDevTV.RTS.Events
{
public struct ActionSelectedEvent : IEvent
    {
        public BaseCommand Action {get;}
        public ActionSelectedEvent(BaseCommand action){Action = action;}
    }
}