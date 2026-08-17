using System.Collections.Generic;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.Commands;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.EventSystems;

namespace GameDevTV.RTS.Player
{
// Unit Selection Bus Logic (mermaid)
// flowchart TD
//     A[PlayerInput.HandleLeftClick] --> B[AbstractCommandable.Select]
//     B-->|Raises|C[Bus UnitSelectedEvent]
//     C-->|Subscribes|D[PlayerInput.HandleUnitSelect]
//     C-->|Subscribes|E[ActionIUI]


public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Rigidbody cameraTarget;
    [SerializeField] private Camera camera;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CameraConfig cameraConfig;
    [SerializeField] private LayerMask selectableLayers;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private LayerMask floorLayers;
    [SerializeField] private RectTransform selectionBox;

    [Header("UI Colours")]
    [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color errorColorTint = Color.red;
    [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color errorColorFresnel = new(4, 1.7f, 0, 2);
    [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color availableToPlaceColorTint = new(0.2f, 0.65f, 1, 2); 
    [SerializeField] [ColorUsage(showAlpha:true, hdr:true)] private Color availableToPlaceColorFresnel = new(4, 1.7f, 0, 2);

    private BaseCommand activeAction;
    private CinemachineFollow cinemachineFollow;
    private float zoomStartTime;
    private float rotationStartTime;
    private float maxRotationAmount;
    private bool wasMouseDownOnUI;
    private Vector3 defaultFollowOffset;
    private List<ISelectable> selectedUnits = new(12); // FORCE!!! a size of 12 for efficiency.
    private Vector2 startClickMousePos;
    private HashSet<AbstractUnit> aliveUnits = new(100); // arb numbers being used here :/
    private HashSet<AbstractUnit> selectionBoxUnits = new(24); // arb numbers being used here :/

    private GameObject ghostInstance;
    private MeshRenderer ghostRenderer;

    // Get hashes of "references" in shadergraph
    private static readonly int TINT = Shader.PropertyToID("_Tint");
    private static readonly int FRESNEL = Shader.PropertyToID("_FresnelColor");

    #region Unity Lifecycle
    private void Awake(){
        if (!cinemachineCamera.TryGetComponent(out cinemachineFollow)){
            Debug.LogError("CinemachineCamera did not have CinemachineFollow. Zoom functionality will not work");
        }
        defaultFollowOffset = cinemachineFollow.FollowOffset;
        maxRotationAmount = Mathf.Abs(cinemachineFollow.FollowOffset.z);
    }

    private void Update(){
        HandlePanning();
        HandleZoom();
        HandleRotation();
        HandleRightClick();
        HandleDragSelect();
        HandleGhost();
    }

    private void OnEnable()
    {
        Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
        Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
        Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        Bus<ActionSelectedEvent>.OnEvent += HandleActionSelected;
    }

    private void OnDisable()
    {
        Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
        Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
        Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        Bus<ActionSelectedEvent>.OnEvent -= HandleActionSelected;
    }

    #endregion

    #region Screen Movement
    private void HandlePanning()
    {
        Vector2 moveAmount = Vector2.zero;

        moveAmount += GetKeyboardMoveAmount();
        moveAmount += GetMousePanMoveAmount();

        cameraTarget.linearVelocity = new Vector3(moveAmount.x, 0f, moveAmount.y);
    }

    private Vector2 GetKeyboardMoveAmount()
    {
        Vector2 moveAmount = Vector2.zero;
        if (Keyboard.current.upArrowKey.isPressed) { moveAmount.y += cameraConfig.KeyboardPanSpeed; }
        if (Keyboard.current.rightArrowKey.isPressed) { moveAmount.x += cameraConfig.KeyboardPanSpeed; }
        if (Keyboard.current.downArrowKey.isPressed) { moveAmount.y -= cameraConfig.KeyboardPanSpeed; }
        if (Keyboard.current.leftArrowKey.isPressed) { moveAmount.x -= cameraConfig.KeyboardPanSpeed; }
        return moveAmount;
    }

    private Vector2 GetMousePanMoveAmount()
    {
        Vector2 moveAmount = Vector2.zero;
        if (!cameraConfig.EnableEdgePan){return moveAmount;}
        
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (mousePosition.y > Screen.height - cameraConfig.MousePanSize) { moveAmount.y += cameraConfig.MousePanSize; }
        if (mousePosition.x > Screen.width - cameraConfig.MousePanSize) { moveAmount.x += cameraConfig.MousePanSize; }
        if (mousePosition.y < cameraConfig.MousePanSize) { moveAmount.y -= cameraConfig.MousePanSize; }
        if (mousePosition.x < cameraConfig.MousePanSize) { moveAmount.x -= cameraConfig.MousePanSize; }

        return moveAmount;
    }

    private void HandleZoom()
    {
        if (ShouldSetZoomStartTime()){
            zoomStartTime = Time.time;
        }

        Vector3 targetFollowOffset;
        
        if (Keyboard.current.endKey.isPressed){
            targetFollowOffset = new Vector3(
                cinemachineFollow.FollowOffset.x,
                cameraConfig.MinZoomDistance,
                cinemachineFollow.FollowOffset.z
            );

        }
        else{
            targetFollowOffset = new Vector3(
                cinemachineFollow.FollowOffset.x,
                defaultFollowOffset.y,
                cinemachineFollow.FollowOffset.z
            );
        }
        float zoomFraction = Mathf.Clamp01((Time.time - zoomStartTime) * cameraConfig.ZoomSpeed); // cool zooming, Christopher.

        cinemachineFollow.FollowOffset = Vector3.Slerp(
            cinemachineFollow.FollowOffset,
            targetFollowOffset,
            zoomFraction
        );

    }

    private void HandleRotation(){
        // works by moving the follow x,z points... not actually rotating the cameraTarget following
        if (ShouldSetRotationStartTime()) { rotationStartTime = Time.time; }

        float rotationFraction = Mathf.Clamp01((Time.time - rotationStartTime) * cameraConfig.RotationSpeed);
        Vector3 targetFollowOffset;

        if (Keyboard.current.pageDownKey.isPressed){
            targetFollowOffset = new Vector3(maxRotationAmount, cinemachineFollow.FollowOffset.y, 0);
        }
        else if (Keyboard.current.pageUpKey.isPressed){
            targetFollowOffset = new Vector3(-maxRotationAmount, cinemachineFollow.FollowOffset.y, 0);
        }
        else{
            targetFollowOffset = new Vector3(defaultFollowOffset.x, cinemachineFollow.FollowOffset.y, defaultFollowOffset.z);
        }

        cinemachineFollow.FollowOffset = Vector3.Slerp(cinemachineFollow.FollowOffset, targetFollowOffset, rotationFraction);

    }

    private bool ShouldSetZoomStartTime(){
        return Keyboard.current.endKey.wasPressedThisFrame || Keyboard.current.endKey.wasReleasedThisFrame;
    }

    private bool ShouldSetRotationStartTime(){
        return Keyboard.current.pageUpKey.wasPressedThisFrame 
            || Keyboard.current.pageUpKey.wasReleasedThisFrame
            || Keyboard.current.pageDownKey.wasPressedThisFrame
            || Keyboard.current.pageDownKey.wasReleasedThisFrame;
    }

    #endregion

    #region Clicking On Things
    private void HandleLeftClick()
    {
        if (camera == null){return; }
        
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasReleasedThisFrame){
            if (activeAction == null
                && Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, selectableLayers)
                && hitInfo.collider.TryGetComponent(out ISelectable selectable))
            {
                DebugLogging.Instance.Message($"REPORT_CLICKS: Left click registered: {hitInfo.collider.name}",DebugLogging.Instance.REPORT_CLICKS);
                DebugLogging.Instance.Message($"REPORT_SELECTION: Selection from HandleLeftClick: {hitInfo.collider.name}",DebugLogging.Instance.REPORT_SELECTION);
                selectable.Select();
            }
            else if (activeAction != null
                // Deal with second LMB click action (ActionBase.RequiresClickToActivate)
                && !EventSystem.current.IsPointerOverGameObject()
                && Physics.Raycast(ray, out hitInfo, float.MaxValue, floorLayers | interactableLayers))
                {
                    DebugLogging.Instance.Message(
                        $"REPORT_CLICKS: Left click registered: {hitInfo.collider.name}",
                        DebugLogging.Instance.REPORT_CLICKS
                    );
                    ActivateAction(hitInfo);
                }
            }
    }

        private void ActivateAction(RaycastHit hitInfo)
        {
            List<AbstractCommandable> commandables = selectedUnits
                .Where((unit) => unit is AbstractCommandable)
                .Cast<AbstractCommandable>()
                .ToList();

            for (int i = 0; i < commandables.Count; i++)
            {
                CommandContext context = new(commandables[i], hitInfo, i);
                // Chris Kurhan's version broke, so he had to remove this... so we have to remove as well. Cheers Chris.
                //if (activeAction.CanHandle(context)){
                    activeAction.Handle(context);
                //}
            }

            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
            }

            activeAction = null;
        }

        private void HandleRightClick()
        {
        // handle right click will take FIRST valid command in command list that can be handled

        if (camera == null){return; }
        if (selectedUnits.Count == 0){return; }
        
        //  || selectedUnits is not IMoveable moveable
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Mouse.current.rightButton.wasReleasedThisFrame
            && Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, floorLayers | interactableLayers)){
                DebugLogging.Instance.Message(
                    $"REPORT_CLICKS: Right click registered: {hitInfo.collider.name}",
                    DebugLogging.Instance.REPORT_CLICKS
                );
            // Find the appropriate command 
            // issue command to units

            List<AbstractUnit> abstractUnits = new List<AbstractUnit>(selectedUnits.Count);
            foreach (ISelectable selectable in selectedUnits){
                if (selectable is AbstractUnit unit) { abstractUnits.Add(unit); 
                }
            }

            int unitCtr = 0;
            foreach (AbstractUnit unit in abstractUnits){
                foreach (ICommand command in GetAvailableCommands(unit)){
                    CommandContext cxt = new CommandContext(unit, hitInfo, unitCtr);
                    if (command.CanHandle(cxt)){
                         command.Handle(cxt);
                         unitCtr +=1;
                         break; // only issue one cmd per unit, and if any, chose first.
                    }
                }
            }
        }
    }

    private List<BaseCommand> GetAvailableCommands(AbstractCommandable unit){
        // this is horrendously dynamic
        List<BaseCommand> availableCommands = new();
        
        // WARNING: we want the priority to depend on order...
        // for this reason, we're currently reading out override commands first for the sake of 
        // the resume building command...

        // Step 1, Add all commands in override menu (except reset O.R.C. command, which is itself an O.R.C. command)
        // Step 1.1 get commands which are only OverrideCommandsCommand's
        OverrideCommandsCommand[] overrideCommandsCommands = unit.AvailableCommands
            .Where(command => command is OverrideCommandsCommand)
            .Cast<OverrideCommandsCommand>()
            .ToArray();
        // Step 1.2 go into overrides command and add all non-ORC commands
        foreach (OverrideCommandsCommand overridesCommandCommand in overrideCommandsCommands){
            availableCommands.AddRange(
                overridesCommandCommand.newCommands
                .Where(command => command is not OverrideCommandsCommand)
            );
        }

        // Step 2. Add commands from unit first menu actions
        availableCommands.AddRange(
            unit.AvailableCommands
            .Where(command => command is not OverrideCommandsCommand)
        );

        return availableCommands;
    }

    private void HandleDragSelect(){
        if (selectionBox == null){return;}

        if (Mouse.current.leftButton.wasPressedThisFrame){
            HandleDragSelect_Start();
        }
        else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame){
            HandleDragSelect_Drag();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame){
            if (!wasMouseDownOnUI && activeAction == null && !Keyboard.current.shiftKey.isPressed) { DeselectAll(); }
            HandleLeftClick(); // This code is rife, Chris
            HandleDragSelect_Stop();
        }
    }

    private void HandleDragSelect_Start(){
        selectionBox.gameObject.SetActive(true);
        startClickMousePos = Mouse.current.position.ReadValue();
        selectionBoxUnits.Clear();
        wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
         
    }

    private void HandleDragSelect_Drag(){
        if (activeAction != null || wasMouseDownOnUI){return;}

        Bounds selectionBoxBounds = ResizeSelectionBox();
        foreach (AbstractUnit unit in aliveUnits)
        {
            Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
            if (selectionBoxBounds.Contains(unitPosition)){
                selectionBoxUnits.Add(unit);
            }
        } 
    }

    private void HandleDragSelect_Stop(){
        selectionBox.gameObject.SetActive(false);
        selectionBox.sizeDelta = Vector2.zero;
        foreach (AbstractUnit unit in selectionBoxUnits) {
            unit.Select();
            DebugLogging.Instance.Message($"REPORT_SELECTION: Selection from HandleDragSelect_Stop: {unit.name}",DebugLogging.Instance.REPORT_SELECTION);
        }
    }

    private void DeselectAll()
    {
        // must not modify selectedUnits list in foreach loop. Grab snapshot array
        ISelectable[] currentlySelected = selectedUnits.ToArray();
        foreach (ISelectable selected in currentlySelected ) {
            selected.Deselect();
        }
    }

    private Bounds ResizeSelectionBox()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        float width = currentMousePos.x - startClickMousePos.x;
        float height = currentMousePos.y - startClickMousePos.y;
        selectionBox.anchoredPosition = startClickMousePos + new Vector2(width, height) / 2; // pivot position is at 0.5
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        return new Bounds (selectionBox.anchoredPosition, selectionBox.sizeDelta);
    }

    private void HandleUnitSelected(UnitSelectedEvent evt){
        if (!selectedUnits.Contains(evt.Unit)) selectedUnits.Add(evt.Unit);
        
    }
    private void HandleUnitDeselected(UnitDeselectedEvent evt) => selectedUnits.Remove(evt.Unit);
    private void HandleUnitSpawn(UnitSpawnEvent evt) => aliveUnits.Add(evt.Unit);
    
    private void HandleUnitDeath(UnitDeathEvent evt){
        selectedUnits.Remove(evt.Unit);
        aliveUnits.Remove(evt.Unit);
    }

    private void HandleActionSelected(ActionSelectedEvent evt){
        DebugLogging.Instance.Message(
            $"REPORT_CLICKS Click registered on {evt.Action.name}",
            DebugLogging.Instance.REPORT_CLICKS
        );
        activeAction = evt.Action;
        if (!activeAction.RequiresClickToActivate){
            ActivateAction(new RaycastHit()); // use dummy raycast hit
        }
        else if (activeAction.GhostPrefab != null){
            // we're placing the ghost
            ghostInstance =  Instantiate(activeAction.GhostPrefab);
            ghostRenderer = ghostInstance.GetComponentInChildren<MeshRenderer>();
        }
    }

    private void HandleGhost(){
        if (activeAction == null || ghostInstance == null) return;
        if (Keyboard.current.escapeKey.wasReleasedThisFrame){
            Destroy(ghostInstance);
            ghostInstance = null;
            activeAction = null;
            return;
        }

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, floorLayers)){
            ghostInstance.transform.position = hitInfo.point;

            bool restrictionsPass =activeAction.AllRestrictionsPass(hitInfo.point);
            ghostRenderer.material.SetColor(TINT, restrictionsPass? availableToPlaceColorTint : errorColorTint);
            ghostRenderer.material.SetColor(FRESNEL, restrictionsPass? availableToPlaceColorFresnel : errorColorFresnel);
        }
    }
    #endregion

}
}