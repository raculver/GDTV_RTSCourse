using System;
using GameDevTV.RTS.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components{

[RequireComponent(typeof (Button))]
public class UIActionButton : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject tooltipText;
    
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
    }

    public void Disable(){
        SetIcon(null);
        button.interactable = false;
        button.onClick.RemoveAllListeners();
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
        // cancel pending invocation
        CancelInvoke(nameof(ShowTooltip));
        HideToolTip();
    }

    private void ShowTooltip() => tooltipText.SetActive(true);
    private void HideToolTip() => tooltipText.SetActive(false);

}
}