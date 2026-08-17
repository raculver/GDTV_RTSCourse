using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.UI.Components
{
    public class Tooltip : MonoBehaviour
    {
        [field: SerializeField][Range(0,2)] public float HoverDelay{get; private set;} = 0.8f;
        [SerializeField] private TextMeshProUGUI textObject;

        public void SetText(string text) => this.textObject.SetText(text);
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}