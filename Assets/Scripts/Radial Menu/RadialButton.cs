using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public Image circle;
    public Image icon;
    public string title;
    public RadialMenu myMenu;
    public float speed = 8f;
    Color defaultColor;
    public Interactable interactable;
    public int selectedOption;

    public void Anim()
    {
        StartCoroutine(AnimateButtonIn());
    }

    IEnumerator AnimateButtonIn()
    {
        transform.localScale = Vector3.zero;
        float timer = 0f;
        while (timer < (1 / speed))
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.one * timer * speed;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }


    IEnumerator AnimateButtonOver()
    {
    //    transform.localScale = Vector3.one;
        float timer = 0f;
        while (timer < (1 / speed))
        {
            timer += Time.deltaTime;
            transform.localScale += new Vector3(.1f, .1f) * timer * 4;
            yield return null;
        }

    }

    IEnumerator AnimateButtonOut()
    {
       // transform.localScale = Vector3.one;
        float timer = 0f;
        while (timer < (1 / speed))
        {
            timer += Time.deltaTime;
            transform.localScale -= new Vector3(.1f, .1f) * timer * 4;
            yield return null;
        }

    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        myMenu.selected = this;

        defaultColor = circle.color;
        circle.color = Color.yellow;
        StartCoroutine(AnimateButtonOver());

        //circle.transform.localScale += new Vector3(.5f, .5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myMenu.selected = null;
        circle.color = defaultColor;
        StartCoroutine(AnimateButtonOut());

        //circle.transform.localScale -= new Vector3(.5f, .5f);
    }
    
  
}
