using UnityEngine;

public class DebugLogging : MonoBehaviour
{
    [Header("=== Debug Toggles ===")]


    [SerializeField] private bool ENABLE_MESSAGES = true;
    [field:SerializeField] public bool REPORT_CLICKS {get; private set;} = false;
    [field:SerializeField] public bool ACTION_GATHER_SUP {get; private set;} = false;
    [field:SerializeField] public bool ACTION_MOVE_GATHER {get; private set;} = false;
    [field:SerializeField] public bool ACTION_FIND_CP {get; private set;} = false;
    [field:SerializeField] public bool ACTION_MOVE_TO_TARGET_POS {get; private set;} = false;
    [field:SerializeField] public bool ACTION_SET_NAV_AVOIDANCE {get; private set;} = false;

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

    public void Message(string message, bool debugCheck)
    {
        if (ENABLE_MESSAGES && debugCheck)
            Debug.Log($"Debug Message: {message}");
    }
}