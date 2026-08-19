using System;
using GameDevTV.RTS.Commands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameDevTV.RTS.UI.Components{

[RequireComponent(typeof (Button))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform mask;
    private RectTransform maskParent;
    private float padding_x;

    private void Awake()
    {
        if (mask == null){
            Debug.LogError($"Progress bar {name} is missing a mask. Progress bar will not work");
            return;
        }
        maskParent = mask.parent.GetComponent<RectTransform>();
        padding_x = mask.offsetMin.x;
    }

    public void SetProgress(float progress){
        if (mask == null || maskParent == null) return;
        Vector2 padding = new (padding_x,0);

        Vector2 parentSize = maskParent.sizeDelta;
        Vector2 targetSize = parentSize - padding * 2;

        targetSize.x *= Mathf.Clamp01(progress);

        mask.offsetMin = padding;
        mask.offsetMax = new Vector2(padding_x + targetSize.x - parentSize.x, 0);
    }
}
}