using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Building Restriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO: ScriptableObject
    {
        [field: SerializeField] public bool MustBeFullyOnNavmesh{get; private set;} = true;
        [field: SerializeField] public int NavMeshAgentTypeId {get; private set;}      
        [field: SerializeField] public float NavMeshTolerance {get; private set;} = 0.1f;

        [field: SerializeField] public Vector3 Extents {get; private set;} = Vector3.one;

        public bool CanPlace(Vector3 position){
            return (!MustBeFullyOnNavmesh) || FourCornersOnNavMesh(position);
        }

        private bool FourCornersOnNavMesh(Vector3 position){
            NavMeshQueryFilter query = new(){areaMask = NavMesh.AllAreas, agentTypeID = NavMeshAgentTypeId};
            return NavMesh.SamplePosition(position + new Vector3( Extents.x, 0,  Extents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3( Extents.x, 0, -Extents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0,  Extents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3(-Extents.x, 0, -Extents.z), out NavMeshHit _, NavMeshTolerance, query);
        }
    }
}