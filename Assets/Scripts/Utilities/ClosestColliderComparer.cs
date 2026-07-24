using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public struct ClosestColliderComparer : IComparer<Collider>{
        private Vector3 positionToCompare;
        
        public ClosestColliderComparer(Vector3 position){
            positionToCompare = position;
        }
        
        public int Compare(Collider x, Collider y){
            // returns < 0 if x should be comes first
            // returns > 0 if y should be comes first
            float xdist = (x.transform.position - positionToCompare).magnitude;
            float ydist = (y.transform.position - positionToCompare).magnitude;
            return xdist.CompareTo(ydist);
        }
    }
}