using UnityEngine;

namespace GameDevTV.RTS.Environment
{
    public class GatherableSupply : MonoBehaviour, IGatherable
    {
        [field: SerializeField] public SupplySO Supply {get; private set;}
        
        public int AmountRemaining{get; private set;}
        public bool IsBusy{get; private set;}

        private void Awake(){
            AmountRemaining = Supply.MaxAmount;
            IsBusy = false;
        }

        public bool BeginGather()
        {
            if (IsBusy) return false;

            IsBusy = true;
            return true;
        }

        public int EndGather()
        {
            IsBusy = false;
            int amountGathered = Mathf.Min(Supply.AmountPerGather, AmountRemaining);
            AmountRemaining -= amountGathered;
            if (AmountRemaining == 0){
                Destroy(gameObject);
            }
            return amountGathered;
        }
    }

}