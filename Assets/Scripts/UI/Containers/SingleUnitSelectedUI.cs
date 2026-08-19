using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.UI.Containers
{
    public class SingleUnitSelectedUI : MonoBehaviour, IUIElement<AbstractUnit>
    {
        [SerializeField] private TextMeshProUGUI unitName;

        public void EnableFor(AbstractUnit item){
            gameObject.SetActive(true);
            unitName.text = item.unitSO.Name;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}