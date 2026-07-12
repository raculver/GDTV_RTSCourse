using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{   
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [SerializeField] private DecalProjector selectionDecal;
        [field: SerializeField] public int Health{get; private set;}

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
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

    }
}