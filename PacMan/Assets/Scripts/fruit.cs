using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class fruit : MonoBehaviour {
    public int value;
    public Sprite sprite;
    public void Start()
    {
        Destroy(gameObject, 10f);
    }
    public int getValue()
    {
        return value;
    }

}
