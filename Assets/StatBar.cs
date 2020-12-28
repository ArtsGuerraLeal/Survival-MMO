using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    public Slider slider;

    public void setValue(int val) {
        slider.value = val;
    }
    public void setMaxValue(int maxVal) {
        slider.maxValue = maxVal;
    }

}
