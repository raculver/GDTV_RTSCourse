using Unity.Behavior;

namespace GameDevTV.RTS.Units
{
    [BlackboardEnum]
    public enum BuildingEventType
    {
         ArrivedAt,
         Begin,
         Cancel, // fully cancel and refund supplies
         Abort,  // can't do right now, do something else.
         Complete
    }
}