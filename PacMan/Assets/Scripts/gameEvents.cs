using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class gameEvents : MonoBehaviour
{
    public static gameEvents current;
    private void Awake()
    {
        current = this;
    }
    public event Action onLifeLost;
    public void lifeLost()
    {
        if(onLifeLost != null)
        {
            
            onLifeLost();
        }
    }
    public event Action onPowerDot;
    public void powerDot()
    {
        if(onPowerDot != null)
        {
            onPowerDot();
        }
    }
    public event Action onLoss;
    public void loss()
    {
        if (onLoss != null)
        {
            onLoss();
        }
    }
}
