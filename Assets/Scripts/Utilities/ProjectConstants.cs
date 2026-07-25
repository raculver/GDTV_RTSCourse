using UnityEngine;

namespace GameDevTV.RTS.Constants{
public static class GameLayers{
    public static readonly LayerDefinition Units = new ("Units", 6);
    public static readonly LayerDefinition Buildings = new ("Buildings", 7);
    public static readonly LayerDefinition Floor = new ("Floor", 8);
    public static readonly LayerDefinition Supplies = new ("Supplies", 9);
}

public class LayerDefinition{
    public string Name {get;}
    public int Index {get;}
    public int Mask {get;}

    public LayerDefinition(string name, int layerNum){
        Name = name;
        Index = layerNum;
        Mask = LayerMask.GetMask(name);
        Validate();
    }

    public bool Validate(){
        if (Index != LayerMask.NameToLayer(Name)){
            Debug.LogError($"Layer mask mismatch with layer: {Name} not at layer index {Index}.");
            return false;
        }
        return true;
    }

    public static implicit operator int(LayerDefinition layerDefinition){
        // implicitly convert to int
        if (layerDefinition == null) return 0;
        return layerDefinition.Mask; 
    }

    public static implicit operator LayerMask(LayerDefinition layerDefinition){
        // implicitly convert to int
        if (layerDefinition == null) return 0;
        return (LayerMask)layerDefinition.Mask; 
    }

}
}