using System;
using System.Collections.Generic;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.UI.Containers;
using GameDevTV.RTS.Units;
using UnityEngine;

/* Mermaid attempt
flowchart LR
    RUI[RuntimeUI]-->|activates|UI["UI.Container.ActionUI"]-->|activates|UIC["UI.Component.UIActionButton"]
    UIC -->|inherits from|IUIE2["IUIElement(ActionBase,UnityAction)"]
    UI -->|inherits from|IUIE1["IUIElement(T)"]
    RUI -->|owns|L_AC["List<_AbstractCommandable_>"]-->AC[AbstractCommandable]
    AB[ActionBase]--> IUIE2
    q[?]-->AB
*/
namespace GameDevTV.RTS.UI
{
public class RuntimeUI:MonoBehaviour{
    [SerializeField] private ActionsUI actionsUI;
    private HashSet<AbstractCommandable> currentlySelected = new(12);

    void OnEnable(){
        Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
    }

    void OnDisable(){
        Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
    }

    //private void HandleUnitSelected(UnitSelectedEvent evt) => currentlySelected.Add(evt.Unit);
    private void HandleUnitSelected(UnitSelectedEvent evt){        
        if(evt.Unit is AbstractCommandable commandable) {
            currentlySelected.Add(commandable);
            actionsUI.EnableFor(currentlySelected);
        }
    }

    private void HandleUnitDeselected(UnitDeselectedEvent evt){
        actionsUI.Disable();

        if(evt.Unit is AbstractCommandable commandable) {
            currentlySelected.Remove(commandable);
            // Show ui with new selection, or if there are none slected, disable entirely.
            if (currentlySelected.Count > 0){
                actionsUI.EnableFor(currentlySelected);
            }
            else{
                actionsUI.Disable();                
            }
        }        
    }
}
}