using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{   
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int CurrentHealth{get; private set;}
        [field: SerializeField] public int MaximumHealth{get; private set;}
        [field: SerializeField] public BaseCommand [] AvailableCommands{get; private set;}
        [field: SerializeField] public AbstractUnitSO unitSO{get; private set;}
        [SerializeField] private DecalProjector selectionDecal;

        private BaseCommand[] initialCommands;
        bool commandsOverridden;
        
        // "virtual means the child classes can override if they need to".
        protected virtual void Start(){
            CurrentHealth = unitSO.Health;
            MaximumHealth = unitSO.Health;
            initialCommands = AvailableCommands;
            commandsOverridden = false;
        }

        public void Select() {
            if (selectionDecal != null){
                selectionDecal.gameObject.SetActive(true);
            }
            
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
            DebugLogging.Instance.Message($"{this.name} Selected.", DebugLogging.Instance.REPORT_SELECTION);
        }

        public void Deselect() {
            if (selectionDecal != null){
                selectionDecal.gameObject.SetActive(false);
            }
            if (commandsOverridden){
                // Reset on delete
                SetCommandsOverride(null);
                commandsOverridden = false;
            }
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
            DebugLogging.Instance.Message($"{this.name} Deselected.", DebugLogging.Instance.REPORT_SELECTION);
        }

        public void SetCommandsOverride(BaseCommand[] commands){
            bool noCommands = commands == null || commands.Length == 0;
            if (noCommands){
                commandsOverridden = false;
                AvailableCommands = initialCommands;
                Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this)); // Mr Kurhan did UnitSelectedEvent. Weird doubley selected unit were causing errors after worker spawn
            }
            else{
                commandsOverridden = true;
                AvailableCommands = commands;
                Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this)); // Mr Kurhan did UnitSelectedEvent. Weird doubley selected units were causing errors after worker spawn
            }
        }

        protected void PaySupplies(SupplyCostSO cost){
            Bus<SupplyEvent>.Raise(new SupplyEvent(-cost.Minerals, cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent(-cost.Gas, cost.GasSO));
        }

        protected void RefundSupplies(SupplyCostSO cost, float fraction = 1.0f){
            Bus<SupplyEvent>.Raise(new SupplyEvent((int)(fraction*cost.Minerals), cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent((int)(fraction*cost.Gas), cost.GasSO));
        }


    }
}