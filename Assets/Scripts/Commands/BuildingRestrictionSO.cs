using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Commands
{
    [CreateAssetMenu(fileName = "Building Restriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO: ScriptableObject
    {
        [field: SerializeField] public Vector3 HitExtents {get; private set;} = Vector3.one;

        [Header("Placement Hit Detection Checks")]
        [field: SerializeField] public float HitRadius {get; private set;} = 1f;
        [field: SerializeField] public LayerMask LayersToCheck {get; private set;}
        [field: SerializeField] public OverlapStyle HitDetectionStyle = OverlapStyle.Sphere;
        
        [Header("NavMesh Checks")]
        [field: SerializeField] public bool MustBeFullyOnNavmesh{get; private set;} = true;
        [field: SerializeField] public int NavMeshAgentTypeId {get; private set;}      
        [field: SerializeField] public float NavMeshTolerance {get; private set;} = 0.1f;

        private Collider[] hitColliders = new Collider[1]; // in our useage here we only care if a single object was hit.

        public bool CanPlace(Vector3 position){
            bool navMeshChecks = (!MustBeFullyOnNavmesh) || FourCornersOnNavMesh(position);
            bool placementHitChecks = GetPlacementHitChecks(position);

            return navMeshChecks && placementHitChecks;
        }

        private bool GetPlacementHitChecks(Vector3 position)
        {
            return HitDetectionStyle switch
            {
                OverlapStyle.Sphere => Physics.OverlapSphereNonAlloc(position, HitRadius, hitColliders, LayersToCheck) == 0,
                OverlapStyle.Box => Physics.OverlapBoxNonAlloc(position, HitExtents, hitColliders, Quaternion.identity, LayersToCheck) == 0,
                _ => throw new System.NotImplementedException()
            };
        }

        private bool FourCornersOnNavMesh(Vector3 position){
            NavMeshQueryFilter query = new(){areaMask = NavMesh.AllAreas, agentTypeID = NavMeshAgentTypeId};
            return NavMesh.SamplePosition(position + new Vector3( HitExtents.x, 0,  HitExtents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3( HitExtents.x, 0, -HitExtents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3(-HitExtents.x, 0,  HitExtents.z), out NavMeshHit _, NavMeshTolerance, query)
                && NavMesh.SamplePosition(position + new Vector3(-HitExtents.x, 0, -HitExtents.z), out NavMeshHit _, NavMeshTolerance, query);
        }

        public enum OverlapStyle{
            Sphere,
            Box
        }
    }
}