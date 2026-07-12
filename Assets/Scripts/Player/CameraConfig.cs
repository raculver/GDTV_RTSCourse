using UnityEngine;

namespace GameDevTV.RTS.Player
{
    [System.Serializable] // Makes discoverable in Unity Inspector
    public class CameraConfig{

        [field: SerializeField] public bool EnableEdgePan { get; private set;} = true;
        [field: SerializeField] public float MousePanSpeed { get; private set;} = 5f;
        [field: SerializeField] public float MousePanSize { get; private set;} = 5f;

        [field: SerializeField] public float KeyboardPanSpeed { get; private set;} = 5f;

        [field: SerializeField] public float ZoomSpeed { get; private set;} = 1f;
        [field: SerializeField] public float RotationSpeed { get; private set;} = 1f; 
        
        [field: SerializeField] public float MinZoomDistance { get; private set;} = 7.5f;        
    }
}