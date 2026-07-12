using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public float AgentRadius => agent.radius;
        private NavMeshAgent agent;

        void Awake(){
            agent = GetComponent<NavMeshAgent>();
        }

        protected override void Start(){
            base.Start(); // call AbstractCommandable implementation of start
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position) {
            agent.SetDestination(position);
        }

    }
}