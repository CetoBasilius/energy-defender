using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{

    public float currentNumber = 0;
    public TextMeshProUGUI counterText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetInt(float number)
    {
        this.currentNumber = number;
        int numberInt = (int) Math.Floor(number);
        this.counterText.text = numberInt.ToString();
    }

    public void Set(string text)
    {
        this.counterText.text = text;
    }
}
