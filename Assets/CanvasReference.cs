using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasReference : MonoBehaviour
{
    public GameObject buildSignScreen;
    public StatBar health;
    public StatBar stamina;
    public StatBar hunger;

    public TextMeshProUGUI[] values;

    public static CanvasReference instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
