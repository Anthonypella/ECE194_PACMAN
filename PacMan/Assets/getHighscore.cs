using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getHighscore : MonoBehaviour
{
    public TextMeshProUGUI hS;
    // Start is called before the first frame update
    void Start()
    {
        hS.text = PlayerPrefs.GetInt("H", 0).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
