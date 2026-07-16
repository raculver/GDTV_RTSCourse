using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using GameDevTV.RTS.UI.Components;
using UnityEngine;
using UnityEngine.Events;

namespace GameDevTV.RTS.UI.Containers{
public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
{
//    private List<ISelectable> currentlySelected = new(24);
    [SerializeField] private UIActionButton[] actionButtons;  

    private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits){
        HashSet<ActionBase> availableCommands = new(9);

        foreach(AbstractCommandable commandable in selectedUnits){
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

    private UnityAction HandleClick(ActionBase action){
        return () => {
            Bus<ActionSelectedEvent>.Raise(new ActionSelectedEvent(action));
        };
    }

    public void EnableFor(HashSet<AbstractCommandable> selectedUnits){
        RefreshButtons(selectedUnits);
    }

    public void Disable(){
        foreach(UIActionButton button in actionButtons) button.Disable();
    }
}
}


// private HashSet<AbstractCommandable> currentlySelected = new(24);