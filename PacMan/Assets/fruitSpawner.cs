using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fruitSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] fruits;
    public SpriteRenderer fruitDisplay;
    bool spawned1 = false;
    bool spawned2 =false;
    int index1;
    int index2;
    public Vector2 spawnLoc;
    manager m;
    void Start()
    {
        m = manager.Instance;
        if(manager.Instance.level < 3)
        {
            index1 = Random.Range(0, 2);
            index2 = Random.Range(0, 2);
        }
        else
        {
            index1 = Random.Range(0, 4);
            index2 = Random.Range(0, 4);
        }
        updateDisplay();
        
    }
    void updateDisplay()
    {
        if (!spawned1)
            fruitDisplay.sprite = fruits[index1].GetComponent<fruit>().sprite;
        else if (!spawned1)
            fruitDisplay.sprite = fruits[index2].GetComponent<fruit>().sprite;
        else
        {
            fruitDisplay.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!spawned1)
        {
            if(m.getDotsLeft() < 220)
            {
                spawned1 = true;
                Instantiate(fruits[index1], spawnLoc, Quaternion.identity);
                updateDisplay();
            }
        }
        else if (!spawned2)
        {
            if (m.getDotsLeft() < 120)
            {
                spawned2 = true;
                Instantiate(fruits[index2], spawnLoc, Quaternion.identity);
                updateDisplay();
            }
        }
    }
}
