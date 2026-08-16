using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;

namespace GameDevTV.RTS.Events
{
public struct SupplyEvent : IEvent{
        
        public SupplySO SuppliedSupplySO{get;}
        public int AmountSupplied{get;}

        public SupplyEvent(int amountSupplied, SupplySO suppliedSupplySO){
            SuppliedSupplySO = suppliedSupplySO;
            AmountSupplied = amountSupplied;
        }
    }
}