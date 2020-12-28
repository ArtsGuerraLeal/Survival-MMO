using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameTime : MonoBehaviour
{
    public float time;
    public float maxTime;
    public float speed;
    public SpriteRenderer darkness;
    public SpriteRenderer blurness;
    public TextMeshProUGUI timeUI;

    // Start is called before the first frame update
    void Start()
    {
        // time = 0;
        float dayNormalized = time % 1f;
        float hours = ((time - time % 60) / 60);
        string hoursString = ((time - time % 60) / 60).ToString("00");
        string minuteString = ((time - hours * 60)).ToString("00");
        timeUI.text = hoursString + ":" + minuteString;
       // timeUI.text = dayNormalized.ToString();
        StartCoroutine(TickTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TickTime() {
        WaitForSeconds wait = new WaitForSeconds(speed);
        float dayNormalized = time % 1f;
        while (true)
        {
            if (time < maxTime)
            {
                time++;
                float hours = ((time - time % 60) / 60);
                string hoursString = ((time - time % 60) / 60).ToString("00");
                string minuteString = ((time - hours * 60)).ToString("00");
                timeUI.text = hoursString + ":" + minuteString;

            }
            else
            {
                time = 0;
                float hours = ((time - time % 60) / 60);
                string hoursString = ((time - time % 60) / 60).ToString("00");
                string minuteString = ((time - hours * 60)).ToString("00");
                timeUI.text = hoursString + ":" + minuteString;

            }

            if (time > 1140 || time < 180)
            {
                if (darkness.color.a <= 1) {
                    Color tmp = darkness.color;
                    tmp.a += .01f;
                    darkness.color = tmp;
                }

                if (blurness.color.a <= .75f)
                {
                    Color tmp2 = blurness.color;
                    tmp2.a += .005f;
                    blurness.color = tmp2;
                }
               
                

            }
            else {
                if (darkness.color.a >= 0)
                {
                    Color tmp = darkness.color;
                    tmp.a -= .01f;
                    darkness.color = tmp;
                }

                if (blurness.color.a >= 0)
                {
                    Color tmp2 = blurness.color;
                    tmp2.a -= .005f;
                    blurness.color = tmp2;
                }


               

            }
            yield return wait;
        }
       


        
    }
}
