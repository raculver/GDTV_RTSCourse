using System;
using GameDevTV.RTS.Behahavior;
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
            graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Stop);
        }

        protected override void Start(){
            base.Start(); // call AbstractCommandable implementation of start
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position) {
            graphAgent.SetVariableValue<Vector3>(BTVariables.BT_TARGET_POSITION, position);
            graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Move);
        }

        public void Stop(){
            graphAgent.SetVariableValue<Enum>(BTVariables.BT_UNIT_COMMAND, UnitCommands.Stop);
        }
    }
}