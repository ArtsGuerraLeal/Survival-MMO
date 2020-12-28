using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragWindow : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerDownHandler
{

    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Image backgroundImage;
    private Color backgroundColor;

    private void Awake()
    {

        if (rectTransform == null) {
            rectTransform = transform.GetComponent<RectTransform>();
        }

        if (backgroundImage == null)
        {
            backgroundImage = transform.GetComponent<Image>();
        }

        if (canvas == null)
        {
            Transform canvasTransform = transform.parent;

            while (canvasTransform != null) {
                canvas = canvasTransform.GetComponent<Canvas>();

                if (canvas != null) {
                    break;
                }
                canvasTransform = canvasTransform.parent;
            }
        }
        backgroundColor = backgroundImage.color;
 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        backgroundColor.a = .4f;
        backgroundImage.color = backgroundColor;

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        backgroundColor.a = 1f;
        backgroundImage.color = backgroundColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();


    }
}
