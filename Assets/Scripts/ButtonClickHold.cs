using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Detect holding down on UI buttons
public class ButtonClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonDown;
    public UnityEvent longClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonDown = false;
    }
    
	void Update () 
    {
        if (buttonDown)
            longClick.Invoke();
    }
}
