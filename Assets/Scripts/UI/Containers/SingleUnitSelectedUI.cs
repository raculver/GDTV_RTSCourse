using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.UI.Containers
{
    public class SingleUnitSelectedUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private TextMeshProUGUI unitName;

        public void EnableFor(AbstractCommandable commandable){
            gameObject.SetActive(true);
            unitName.text = commandable.unitSO.Name;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}