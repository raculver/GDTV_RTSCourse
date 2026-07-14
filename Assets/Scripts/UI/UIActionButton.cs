using UnityEngine;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI{
public class UIActionButton : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetIcon(Sprite icon){
        if (icon == null){
            Debug.Log("Sprite Disabled");
            this.icon.enabled = false;
        }
        else {
            this.icon.sprite = icon;    
            this.icon.enabled = true;
            Debug.Log($"Sprite Enabled: {icon.name}");
        }
    }
}
}