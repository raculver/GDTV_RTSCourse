using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.RTS.UI{
public class ActionsUI : MonoBehaviour
{
//    private List<ISelectable> currentlySelected = new(24);
    [SerializeField] private UIActionButton[] actionButtons;
    private HashSet<AbstractCommandable> currentlySelected = new(24);

    void Start(){
        foreach(UIActionButton button in actionButtons) button.Disable();
    }

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
        }
        RefreshButtons();
    }

    private void HandleUnitDeselected(UnitDeselectedEvent evt){
        if(evt.Unit is AbstractCommandable commandable) {
            currentlySelected.Remove(commandable);
        }
        RefreshButtons();
    }

    private void RefreshButtons(){
        HashSet<ActionBase> availableCommands = new(9);

        foreach(AbstractCommandable commandable in currentlySelected){
            availableCommands.UnionWith(commandable.AvailableCommands);
        }

        for(int i= 0; i < actionButtons.Length; i++){
            ActionBase actionForSlot = availableCommands.FirstOrDefault(action => action.Slot == i); // use lambda function
            if (actionForSlot != null){
                actionButtons[i].EnableFor(actionForSlot, HandleClick(actionForSlot));
            }
            else{
                actionButtons[i].Disable();
            }
        }
    }

    private UnityAction HandleClick(ActionBase action)
    {
        return () => {
            Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(action));
        };
    }
    }
}