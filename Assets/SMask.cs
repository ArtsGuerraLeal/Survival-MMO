using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMask : MonoBehaviour
{
    [Range(0.05f,0.2f)]
    public float flickTime;
    [Range(0.02f, 0.09f)]
    public float addSize;


    float timer = 0;
    private bool bigger = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > flickTime) {

            if (bigger)
            {
                transform.localScale = new Vector3(transform.localScale.x + addSize, transform.localScale.y + addSize, transform.localScale.z);

            }
            else
            {

                transform.localScale = new Vector3(transform.localScale.x - addSize, transform.localScale.y - addSize, transform.localScale.z);
            }

            timer = 0;
            bigger =! bigger;

        }
    }
}
