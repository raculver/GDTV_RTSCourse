using System.Collections.Generic;
using GameDevTV.RTS.Units;
using UnityEngine;

namespace GameDevTV.RTS.Utilities
{
    public struct ClosestCommandPostComparer : IComparer<BaseBuilding>{
        private Vector3 positionToCompare;
        
        public ClosestCommandPostComparer(Vector3 position){
            positionToCompare = position;
        }
        
        public int Compare(BaseBuilding x, BaseBuilding y){
            // returns < 0 if x should be comes first
            // returns > 0 if y should be comes first
            float xdist = (x.transform.position - positionToCompare).magnitude;
            float ydist = (y.transform.position - positionToCompare).magnitude;
            return xdist.CompareTo(ydist);
        }
    }
}