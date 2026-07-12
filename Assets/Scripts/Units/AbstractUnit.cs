using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.RTS.Units
{

    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : MonoBehaviour, ISelectable, IMoveable
    {
        [SerializeField] DecalProjector selectionDecal;
        public float AgentRadius => agent.radius;
        private NavMeshAgent agent;

        void Awake(){
            agent = GetComponent<NavMeshAgent>();
        }

        void Start(){
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
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
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void MoveTo(Vector3 position) {
            agent.SetDestination(position);
        }

    }
}