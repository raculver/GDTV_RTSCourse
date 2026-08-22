using GameDevTV.RTS.Units;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace GameDevTV.RTS.Behahavior{

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Building Event Channel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Building Event Channel", message: "[Self] [BuildingEventType] on [BaseBuilding] .", category: "Events", id: "2e78e38a6c5afbba83032f5a52da8445")]
public sealed partial class BuildingEventChannel : EventChannel<GameObject, BuildingEventType, BaseBuilding> { }

}