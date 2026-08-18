using System.Security.Cryptography;
using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.UI.Components
{
    public class Tooltip : MonoBehaviour
    {
        [field: SerializeField] public RectTransform TransformObject {get; private set;}
        [field: SerializeField][Range(0,2)] public float HoverDelay{get; private set;} = 0.8f;
        [SerializeField] private TextMeshProUGUI textObject;

        private float xPadding = 50;
        private float yPadding = 20;

        private void Awake() => TransformObject = GetComponent<RectTransform>();

        public void SetText(string text){
            this.textObject.SetText(text);
            Vector2 preferredSize = textObject.GetPreferredValues(); // auto fit transform
            TransformObject.sizeDelta = new Vector2(preferredSize.x + xPadding, preferredSize.y + yPadding);
        }
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}