using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // Continuous polling for movement
    public bool IsPressed { get; private set; }

    // Event-driven actions for single presses (Jump, Shoot)
    public event Action OnButtonDown;
    public event Action OnButtonUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnButtonDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Only trigger if it's currently pressed to avoid double-firing 
        // if OnPointerExit already handled it.
        if (IsPressed)
        {
            ResetButton();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // If the player's finger slides off the button, cancel the press
        if (IsPressed)
        {
            ResetButton();
        }
    }

    // A helper method to keep our code DRY (Don't Repeat Yourself)
    private void ResetButton()
    {
        IsPressed = false;
        OnButtonUp?.Invoke();
    }

}