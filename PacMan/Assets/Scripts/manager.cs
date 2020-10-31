using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class manager : MonoBehaviour
{
    public static manager Instance;

    public int pacDotNum = 10;
    [SerializeField]
    private int currentPacDotNum;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI levelText;
    public GameObject text;
    public float[] frightendLength;
    public float[] scatterLength;
    public float[] chaseLength;
    public bool isFrightened = false;
    int ghostsEatenThisRound = 0;
    private int Score = 0;
    public int level = 1;
    private int levelCap = 255;
    public int Lives = 3;
    bool canLoseLives = true;
    public Transform p;
    AudioManager audio;
    int highSchore = 0;
    // Start is called before the first frame update
    void Awake()
    {
       // DontDestroyOnLoad(this.gameObject);
      /*  
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        */



        
        Instance = this;

    }
    public int getDotsLeft()
    {
        return currentPacDotNum;
    }
    public void Start()
    {
        //Time.timeScale = 1;

        gameEvents.current.onPowerDot += startFrighten;
        currentPacDotNum = pacDotNum;
        audio = AudioManager.Instance;
        loadData();
        updateUI();
        Debug.Log(Lives);
        //p = GameObject.Find("pacman").transform;
    }
    IEnumerator cantDie()
    {
        canLoseLives = false;
        yield return new WaitForSeconds(.2f);
        canLoseLives = true;
        
    }
    public void eatGhost()
    {
        addScore((int)Mathf.Pow(2f, ghostsEatenThisRound) * 200);
        ghostsEatenThisRound++;
        audio.play("eatGhost");
    }
    public void startFrighten()
    {
        StartCoroutine(fright());
    }
    public IEnumerator fright()
    {
        isFrightened = true;
        yield return new WaitForSeconds(getFrightTime());
        isFrightened = false;
        ghostsEatenThisRound = 0;
    }
    public float getFrightTime()
    {
        eatPacdot();
        float fLength;
        if (frightendLength.Length < level)
        {
            fLength = frightendLength[frightendLength.Length - 1];
        }
        else
        {
            fLength = frightendLength[level];
        }
        return fLength;
    }
    public float getChaseTime()
    {
        float fLength;
        if (chaseLength.Length < level)
        {
            fLength = chaseLength[chaseLength.Length - 1];
        }
        else
        {
            fLength = chaseLength[level];
        }
        return fLength;

    }
    public float getScatterTime()
    {
        float fLength;
        if (scatterLength.Length < level)
        {
            fLength = scatterLength[scatterLength.Length - 1];
        }
        else
        {
            fLength = scatterLength[level];
        }
        return fLength;
    }
    public void resetLevel(bool won)
    {
        
        if (won)
        {
            level++;
            
            saveData(true);
            if(level > levelCap)
            {
                //winScreen
            }
            SceneManager.LoadScene(1);
        }
        else
        {
            AudioManager.Instance.play("death");

            
            
            
            saveData(false);
            SceneManager.LoadScene(0);

        }
        
        
    }
    public void saveData(bool won)
    {
        if (won)
        {
            PlayerPrefs.SetInt("L", level);
            PlayerPrefs.SetInt("S", Score);            
            PlayerPrefs.SetInt("H", Mathf.Max(highSchore,Score));
           
        }
        else
        {
            PlayerPrefs.SetInt("L", 1);
            PlayerPrefs.SetInt("S", 0);
        }
        
    }
    public void loadData()
    {
        Score = PlayerPrefs.GetInt("S");
        level = PlayerPrefs.GetInt("L");
        highSchore = PlayerPrefs.GetInt("H");
        Lives = 3;
    }
    public void updateUI()
    {
        scoreText.text = Score.ToString();
        levelText.text = level.ToString();
        lifeText.text = Lives.ToString();
    }
    public void lostLife()
    {
        if (canLoseLives)
        {
            AudioManager.Instance.play("death");
            StartCoroutine(cantDie());
            Lives--;
            Debug.Log(Lives + "Lost life");
            updateUI();
            if (Lives > 0)
            {
                gameEvents.current.lifeLost();
            }
            else
            {
                resetLevel(false);
            }
        }
        
    }
    
    public void eatPacdot()
    {
        //audio.play("eat");
        currentPacDotNum--;
        addScore(10);
        if(currentPacDotNum <= 0)
        {
            resetLevel(true);
        }
        //SceneManager.LoadScene(0);
    }
    public void addScore(int scoreAdd)
    {
        Score += scoreAdd;
        highSchore = Mathf.Max(highSchore, Score);
        saveData(true);
        if(scoreAdd > 10)
        {
            audio.play("eatFruit");
            Instantiate(text, (Vector2)p.position, Quaternion.identity).GetComponent<TextMeshPro>().text = scoreAdd.ToString();
        }
        updateUI();
        //scoreText.text = Score.ToString();
    }
    // Update is called once per frame
    
}
