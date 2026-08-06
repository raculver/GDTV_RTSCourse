using UnityEngine;

namespace GameDevTV.RTS.Units
{
    
public interface IBuildingBuilder
{
    public GameObject Build(BuildingSO building, Vector3 targetLocation);
}

}