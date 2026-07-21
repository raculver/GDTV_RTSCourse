using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public float AgentRadius => navAgent.radius;
     
        private NavMeshAgent navAgent;
        private BehaviorGraphAgent graphAgent;

        void Awake(){
            navAgent = GetComponent<NavMeshAgent>();
            graphAgent = GetComponent<BehaviorGraphAgent>();
            MoveTo(transform.position); // putting this here otherwise the unit runs towards the origin for a split second.
        }

        protected override void Start(){
            base.Start(); // call AbstractCommandable implementation of start
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position) {
            graphAgent.SetVariableValue<Vector3>("TargetLocation", position);
        }
    }
}