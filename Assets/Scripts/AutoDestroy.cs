using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Inventory")) {
            Destroy(this.gameObject);

        }

        if (Input.GetButton("Equipment"))
        {
            Destroy(this.gameObject);

        }

        Destroy(this.gameObject, 10);
    }
}
