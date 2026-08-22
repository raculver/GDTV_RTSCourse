using GameDevTV.RTS.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Containers
{
    public class UnitIconUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI healthText;
        
        private const string HEALTH_TEXT_FORMAT = "{0} / {1}";
        private AbstractCommandable commandable;

        public void EnableFor(AbstractCommandable ac)
        {
            commandable = ac;

            gameObject.SetActive(true);
            icon.sprite = commandable.unitSO.Icon;
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, commandable.CurrentHealth, commandable.MaximumHealth));

            commandable.OnHealthUpdated -= OnHealthUpdated;
            commandable.OnHealthUpdated += OnHealthUpdated;
        }

        public void Disable()
        {
            if (commandable != null){
                commandable.OnHealthUpdated -= OnHealthUpdated;
                commandable = null;
            }
            gameObject.SetActive(false);
        }

        public void OnHealthUpdated(AbstractCommandable commandable, int _, int currentHealth){
            // rife
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, currentHealth, commandable.MaximumHealth));
        }
    }
}
