using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool draggingLeft = false;
    private bool draggingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
        delta *= -1; // natural dragging instinct
        if (draggingLeft && eventData.button == PointerEventData.InputButton.Left)
        {
            CameraDriver.current.orbit(delta);
        }
        else if (draggingRight && eventData.button == PointerEventData.InputButton.Right)
        {
            CameraDriver.current.pan(delta * 0.01f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            draggingLeft = true;
        else if (eventData.button == PointerEventData.InputButton.Right)
            draggingRight = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            draggingLeft = false;
        else if (eventData.button == PointerEventData.InputButton.Right)
            draggingRight = false;
    }
}
