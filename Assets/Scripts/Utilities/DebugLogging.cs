using System;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using UnityEngine;

public class DebugLogging : MonoBehaviour
{
    [Header("Debug Message Toggles")]
    [SerializeField] private bool ENABLE_MESSAGES = true;
    [field:SerializeField] public bool REPORT_CLICKS {get; private set;} = false;
    [field:SerializeField] public bool ACTION_GATHER_SUP {get; private set;} = false;
    [field:SerializeField] public bool ACTION_MOVE_GATHER {get; private set;} = false;
    [field:SerializeField] public bool ACTION_FIND_CP {get; private set;} = false;
    [field:SerializeField] public bool ACTION_MOVE_TO_TARGET_POS {get; private set;} = false;
    [field:SerializeField] public bool ACTION_SET_NAV_AVOIDANCE {get; private set;} = false;
    [field:SerializeField] public bool BUILDING_BASEBUILDING {get; private set;} = false;
    [field:SerializeField] public bool REPORT_SELECTION {get; private set;} = false;

    // Singleton-style access (easy to get from anywhere)
    public static DebugLogging Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);   // survives scene loads
        }
        else{
            Destroy(gameObject);
        }
    }

    private void OnEnable(){
        //Bus<SupplyEvent>.OnEvent += HandleBusEvent;
        //Bus<UnitSelectedEvent>.OnEvent += HandleBusEvent;
        //Bus<UnitDeselectedEvent>.OnEvent += HandleBusEvent;
    }

    private void OnDisable(){
        //Bus<SupplyEvent>.OnEvent -= HandleBusEvent;
        //Bus<UnitSelectedEvent>.OnEvent -= HandleBusEvent;
        //Bus<UnitDeselectedEvent>.OnEvent -= HandleBusEvent;
    }

    public void Message(string message, bool debugCheck)
    {
        if (ENABLE_MESSAGES && debugCheck)
            Debug.Log($"Debug Message: {message}");
    }

    private void HandleBusEvent<T>(T args)
    {
        if (!ENABLE_MESSAGES) return;
        Debug.Log($"BusEvent raised with args: {args}");
    }
}