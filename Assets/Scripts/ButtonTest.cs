using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClicked);
            Debug.Log("Button Listening");
        }
        else
        {
            Debug.LogError("No Button component on this object!", this);
        }
    }

    public void OnButtonClicked()
    {
        Debug.Log("YOU CLICKED ME!");
    }
}