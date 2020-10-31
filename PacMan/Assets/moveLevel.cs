using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class moveLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void mooveLVL(int lvl)
    {
        SceneManager.LoadScene(lvl);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
