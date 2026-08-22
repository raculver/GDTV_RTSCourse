using System;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{   
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int CurrentHealth{get; protected set;}
        [field: SerializeField] public int MaximumHealth{get; protected set;}
        [field: SerializeField] public BaseCommand [] AvailableCommands{get; private set;}
        [field: SerializeField] public AbstractUnitSO unitSO{get; private set;}
        [field: SerializeField] public bool IsSelected{get; protected set;}
        [SerializeField] private DecalProjector selectionDecal;

        private BaseCommand[] initialCommands;
        bool commandsOverridden;

        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHeath, int newHealth);
        public event HealthUpdatedEvent OnHealthUpdated;
        
        // "virtual means the child classes can override if they need to".
        protected virtual void Start(){
            initialCommands = AvailableCommands;
            commandsOverridden = false;
        }

        public void Select() {
            if (selectionDecal != null){
                selectionDecal.gameObject.SetActive(true);
            }
            IsSelected = true;
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
            
            // unit might be destroyed while selected
            if (this != null)
            {
                DebugLogging.Instance.Message($"{this.name} Deselected.", DebugLogging.Instance.REPORT_SELECTION);
            }
            IsSelected = false;
        }

        public void SetCommandsOverride(BaseCommand[] commands){
            bool noCommands = commands == null || commands.Length == 0;
            if (noCommands){
                commandsOverridden = false;
                AvailableCommands = initialCommands;
            }
            else{
                commandsOverridden = true;
                AvailableCommands = commands;
            }

            if (IsSelected)
            {
                Bus<ActionsUIUpdateEvent>.Raise(new ActionsUIUpdateEvent(this)); // Mr Kurhan did UnitSelectedEvent. Weird doubley selected unit were causing errors after worker spawn
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

        public void Heal(int amount)
        {
            int lastHeath = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaximumHealth);
            OnHealthUpdated?.Invoke(this, lastHeath, CurrentHealth);
        }

    }
}