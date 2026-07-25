using GameDevTV.RTS.Environment;
using GameDevTV.RTS.Units;
using UnityEngine;

public class DebugAutoActions : MonoBehaviour
{
    [SerializeField] Worker[] workers;
    [SerializeField] GatherableSupply gathSup;

    void Start(){
        foreach (Worker worker in workers) worker.Gather(gathSup);
    }
}
