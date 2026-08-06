using GameDevTV.RTS.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components{

[RequireComponent(typeof(Button))]
public class UIBuildQueueButton : MonoBehaviour, IUIElement<AbstractUnitSO, UnityAction>
{
    [SerializeField] private Image icon;
    private Button button;

    private void Awake(){
        button = GetComponent<Button>();
        Disable(); 
    }

    public void EnableFor(AbstractUnitSO item, UnityAction callback)
    {
        button.onClick.RemoveAllListeners(); // Rife.
        button.interactable = true;
        button.onClick.AddListener(callback);
        icon.gameObject.SetActive(true);
        icon.sprite = item.Icon;
    }
    
    public void Disable()
    {
        button.interactable = false;        
        button.onClick.RemoveAllListeners();
        icon.gameObject.SetActive(false);
    }
}
}