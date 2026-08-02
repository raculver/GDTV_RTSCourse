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
        [field: SerializeField] public ActionBase [] AvailableCommands{get; private set;}
        [field: SerializeField] public UnitSO unitSO{get; private set;}
        [SerializeField] private DecalProjector selectionDecal;

        private ActionBase[] initialCmmands;
        
        // "virtual means the child classes can override if they need to".
        protected virtual void Start(){
            CurrentHealth = unitSO.Health;
            MaximumHealth = unitSO.Health;
            initialCmmands = AvailableCommands;
        }

        public void Select() {
            if (selectionDecal != null){
                selectionDecal.gameObject.SetActive(true);
            }
            
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void Deselect() {
            if (selectionDecal != null){
                selectionDecal.gameObject.SetActive(false);
            }
            SetCommandsOverride(null);
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void SetCommandsOverride(ActionBase[] commands){
            AvailableCommands = (commands == null || commands.Length == 0) ? initialCmmands : commands;            
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); // Subscribed by RuntimeUI
        }

    }
}