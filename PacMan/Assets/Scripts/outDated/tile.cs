using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour
{
    public bool isAccesible = true;
    public bool isIntersection = false;
    public bool edge = false;
    
    private void Start()
    {
        if (isAccesible)
        {
            GetComponent<SpriteRenderer>().color = Color.red;

        }
        if (isIntersection)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
