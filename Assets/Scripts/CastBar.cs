using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    [SerializeField]
    private bool castRequest, castSuccess, castInProgress;
    public float castTime = 1f;
    public float castStartTime;
    public Slider slider;
    public Image image;
    public bool castFinished;
    public static CastBar instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    CallCast(1);
        //}
    
        if (castRequest)
        {
            ProgressSlider();

            //if (Input.GetMouseButtonUp(0))
            //{
            //    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            //    CastFail();
            //}
        }
        else {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        }
    }

    public void CallCast(float i) {
        castTime = i;
        if (!castInProgress)
        {
            castFinished = false;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            StartCoroutine(Gather());
        }
    }

    public void EndCast() {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        CastFail();
    }

    private void ProgressSlider()
    {
        float timePassed = Time.time - castStartTime;
        float percentComplete = timePassed / castTime;
        slider.value = percentComplete;
        
    }

    private IEnumerator Cast() {
        castInProgress = true;

        RequestCast();

        yield return new WaitUntil(() => castRequest == false);

        if (castSuccess)
        {
            Debug.Log("Cast Successful");
            castFinished = true;
           // image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        else {
            Debug.Log("Cast Was Unsuccessful");

        }

        slider.value = 0;
        castInProgress = false;

    }

    private IEnumerator Gather() {
        castInProgress = true;

        RequestCast();

        yield return new WaitUntil(() => castRequest == false);

        if (castSuccess)
        {
            Debug.Log("Cast Successful");
            CastEnd();
        }
        else
        {
            Debug.Log("Cast Was Unsuccessful");

        }

        slider.value = 0;
        castInProgress = false;
    }

    private void RequestCast()
    {
        castRequest = true;
        castSuccess = false;
        slider.value = 0;
        castStartTime = Time.time;
        Invoke("CastSuccess", castTime);
        

    }

    private void CastSuccess()
    {
        castRequest = false;
        castSuccess = true;

    }

    private void CastEnd()
    {
        castRequest = false;
        castSuccess = false;
        CancelInvoke("CastSuccess");
    }

    private void CastFail()
    {
        castRequest = false;
        castSuccess = false;
        CancelInvoke("CastSuccess");
        
    }
}
