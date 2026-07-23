using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

namespace GameDevTV.RTS.Behahavior
{

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Agent Avoidance", story: "[Agent] uses nav avoidance quality [AvoidanceQuality] .", category: "Action/Navigation", id: "876852ddc035b08917769b5b305c2d41")]
public partial class SetAgentAvoidanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> AvoidanceQuality;

    NavMeshAgent navMeshAgent;

    protected override Status OnStart()
    {
        Agent.Value.TryGetComponent(out navMeshAgent);
        if (navMeshAgent == null){
            DebugLogging.Instance.Message(
                $"ACTION_SET_NAV_AVOIDANCE: Failed to find NavMeshAgent on {Agent}",
                DebugLogging.Instance.ACTION_SET_NAV_AVOIDANCE
            );
            return Status.Failure;
        }
        if (!Enum.IsDefined(typeof(ObstacleAvoidanceType), AvoidanceQuality.Value)){
            DebugLogging.Instance.Message(
                $"ACTION_SET_NAV_AVOIDANCE: Failed to set avoidance quality {AvoidanceQuality}",
                DebugLogging.Instance.ACTION_SET_NAV_AVOIDANCE
            );
            return Status.Failure;
        }

        navMeshAgent.obstacleAvoidanceType = (ObstacleAvoidanceType)AvoidanceQuality.Value;
        return Status.Success;
    }
}

}