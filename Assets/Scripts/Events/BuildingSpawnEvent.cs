
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Units;

namespace GameDevTV.RTS.Events{

    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building {get; private set;}  

        public BuildingSpawnEvent(BaseBuilding building)
        {
            Building = building;
        }
    }
}