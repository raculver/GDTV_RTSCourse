using System;
using GameDevTV.RTS.Commands;
using GameDevTV.RTS.Units;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components{

[RequireComponent(typeof (Button))]
public class UIActionButton : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Tooltip tooltip;
    
    private Button button;
    private float tooltipWaitTime { get;} = 0.8f;

    void Awake(){
        button = GetComponent<Button>();
        Disable();
    }

    public void EnableFor(BaseCommand action, UnityAction onClick){
        SetIcon(action.Icon);
        bool commandLockedOut = action.IsLocked(new CommandContext());
        button.interactable = !commandLockedOut;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
        
        // Deal with tooltip
        if (tooltip != null) tooltip.SetText(GetTooltipText(action));
    }

    public void Disable(){
        SetIcon(null);
        button.interactable = false;
        button.onClick.RemoveAllListeners();
        
        // Deal with tooltip
        HideTooltip();
    }

    private void SetIcon(Sprite icon){
        if (icon == null){
            this.icon.enabled = false;
        }   
        else {
            this.icon.sprite = icon;    
            this.icon.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        // invoke calls after a certain delay
        Invoke(nameof(ShowTooltip), tooltipWaitTime);
    }

    public void OnPointerExit(PointerEventData eventData){
        HideTooltip();
    }

    private void ShowTooltip(){
        if (tooltip != null) tooltip.Show();
    }

    private void HideTooltip(){
        if (tooltip != null) tooltip.Hide();
        CancelInvoke(nameof(ShowTooltip));
    }

    private string GetTooltipText(BaseCommand command){
        string tooltipText = command.CommandName;         

        SupplyCostSO supplyCost = null;
        if (command is BuildBuildingCommand buildBuildingCommand){
            supplyCost = buildBuildingCommand.BuildingType.Cost;
        }
        else if (command is BuildUnitCommand buildUnitCommand){
            supplyCost = buildUnitCommand.UnitToBuild.Cost;
        }
        if (supplyCost != null){
            if (supplyCost.Minerals > 0){
                tooltipText += $"\n{supplyCost.Minerals} Minerals";
            }
            if (supplyCost.Gas > 0){
                tooltipText += $"\n{supplyCost.Gas} Gas";
            }
        }
        return tooltipText;

    }

}
}