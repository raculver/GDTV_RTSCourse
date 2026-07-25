using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;
using UnityEngine;

public class DebugAutoActions : MonoBehaviour
{
    [SerializeField] Worker worker;
    [SerializeField] GatherableSupply gathSup;

    void Start(){
        worker.Gather(gathSup);
    }
}
