using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostTweenMove : MonoBehaviour
{
    public float tweenTime;
    public float circleCastWidth = .2f;
    public float circleCastLength = .5f;
    public float tweenResolution = .5f;
    public LayerMask ghostLayer;
    private Transform pacman;
    public Vector2 targetPoint;
    public float DirectionCalculationClosenessMult = 2f;
    private Animator anim;
    
    public Vector2 scaredTargetPos;
    public Vector2 resurectPos;
    private Vector2 startPos;
    float frightendTweenTime;
    //[SerializeField]
    Vector2 currentDir;
    //[SerializeField]
    Vector2 nextDir;
    bool turnAroundOnNextTween = false;
    bool changetarget = false;
    bool moving = false;
    public State myState;
    public enum State
    {
        chase,
        scatter,
        dead,
        frightened,
        AFK,
    };
    // Start is called before the first frame update
    void Start()
    {
        gameEvents.current.onLoss += loss;
        gameEvents.current.onLifeLost += onPacmanLifeLost;
        gameEvents.current.onPowerDot += scare;
        startPos = getRoundedPos();
        StartCoroutine(stateChanger());
        //myState = State.scatter;
        pacman = GameObject.Find("pacman").transform;
        anim = GetComponent<Animator>();
        targetPoint = pacman.position;
        currentDir = Vector2.right;
        nextDir = currentDir;
        frightendTweenTime = tweenTime * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving == false && nextDir != Vector2.zero && myState != State.AFK)
        {
            endTween();
        }
        if (!changetarget)
        {
            if(manager.Instance.getDotsLeft() < 190)
            {
                changetarget = true;
            }
        }
    }
    void tween(Vector2 dir)
    {
        setAnim(dir);
        if(myState != State.frightened)
        {
            if (changetarget)
            {
                LeanTween.move(gameObject, getRoundedPos() + dir, tweenTime/1.05f).setOnComplete(endTween);
            }
            else
            {
                LeanTween.move(gameObject, getRoundedPos() + dir, tweenTime / 1.05f).setOnComplete(endTween);
            }
        }
        
        else
        {
            LeanTween.move(gameObject, getRoundedPos() + dir, frightendTweenTime).setOnComplete(endTween);
        }
        moving = true;
    }
    public void scare()
    {
        myState = State.frightened;
        turnAroundOnNextTween = true;
        //nextDir = calcNewDirScared();
        setAnim(true);
        StartCoroutine(scared());


    }
    public void loss()
    {
        moving = true;
        LeanTween.cancel(gameObject);
    }
    public IEnumerator scared()
    {
        yield return new WaitForSeconds(manager.Instance.getFrightTime());
        if (!dead())
        {
            myState = State.chase;

            setAnim(false);
        }
        
    }
    public IEnumerator stateChanger()
    {
        if (!dead())
        {
            myState = State.scatter;
            setAnim(currentDir);
        }
        yield return new WaitForSeconds(manager.Instance.getScatterTime());
        if (!dead())
        {
            myState = State.chase;
            
        }
        yield return new WaitForSeconds(manager.Instance.getChaseTime());
        if (!dead())
        {
            myState = State.scatter;
            LeanTween.cancel(gameObject);
            transform.position = getRoundedPos();
            moving = false;
            tween(calcNewDirScared());
        }
           
        
        yield return new WaitForSeconds(manager.Instance.getScatterTime());
        if (!dead())
        {
            myState = State.chase;
           // LeanTween.cancel(gameObject);
            //transform.position = getRoundedPos();
           // moving = false;
            //tween(calcNewDirScared());
        }
        yield return new WaitForSeconds(manager.Instance.getChaseTime());
        if (!dead())
        {
            myState = State.scatter;
            LeanTween.cancel(gameObject);
            transform.position = getRoundedPos();
            moving = false;
            tween(calcNewDirScared());
        }
        yield return new WaitForSeconds(manager.Instance.getScatterTime());
        if (!dead())
            myState = State.chase;
    }


    bool dead()
    {
        return (myState == State.dead);
    }

    public void onPacmanLifeLost()
    {
        moving = false;
        LeanTween.cancel(this.gameObject);
        transform.position = getRoundedPos();
        transform.position = startPos;
        myState = State.chase;
        currentDir = Vector2.right;
        gameObject.layer = 9;
        setAnimDead(false);
        nextDir = currentDir;
    }
    void endTween()
    {
        switch (myState)
        {
            case State.chase:
                targetPoint = pacman.position;
                break;
            case State.scatter:
                if(!changetarget)
                targetPoint = scaredTargetPos;
                else
                {
                    targetPoint = pacman.position;
                }

                break;
            case State.dead:
                targetPoint = resurectPos;
                Debug.Log("Died");
                break;
            case State.frightened:
                targetPoint = (Vector2)transform.position + Random.insideUnitCircle * 2;
                break;

        }

        if (myState == State.frightened)
        {
            nextDir = calcNewDir();
        }

        else
        {
            nextDir = calcNewDir();
        }


        if (nextDir != Vector2.zero && valid(nextDir))
        {

            tween(nextDir);
            currentDir = nextDir;
            nextDir = Vector2.zero;
            moving = true;
        }
        else if (valid(currentDir))
        {
            Debug.Log("Trying to move");
            tween(currentDir);
            moving = true;
        }
        else
        {
            moving = false;
        }
    }
    bool valid(Vector2 dir)
    {

        Vector2 pos = transform.position;
        //Debug.DrawRay((pos + dir * circleCastLength), pos-(pos + dir * circleCastLength),Color.white,1f);
        RaycastHit2D hit = Physics2D.Linecast(pos + dir * circleCastLength, pos, ghostLayer);
        if(hit.collider.CompareTag("Gate") && dead())
        {
            respawn();
        }

        return (hit.collider.CompareTag("Ghost") || hit.collider.CompareTag("Pacman"));

    }
    Vector2 calcNewDir()
    {
        if (turnAroundOnNextTween)
        {
            turnAroundOnNextTween = false;
            return calcNewDirScared();
        }
        else
        {
            List<Vector2> validDirection = new List<Vector2>();
            if (valid(Vector2.right))
            {
                validDirection.Add(Vector2.right);
            }
            if (valid(Vector2.up))
            {
                validDirection.Add(Vector2.up);
            }
            if (valid(Vector2.down))
            {
                validDirection.Add(Vector2.down);
            }
            if (valid(Vector2.left))
            {
                validDirection.Add(Vector2.left);
            }

            for (int i = 0; i < validDirection.Count; i++)
            {
                if (validDirection[i] == -currentDir)
                {
                    // Debug.Log(validDirection[i] + " is opposite direction");
                    validDirection.RemoveAt(i);
                }
            }

            if (validDirection.Count > 1)
            {

                Vector2 pos = transform.position;
                int minDistanceIndex = 0;
                float minDist = ((Vector2)transform.position - targetPoint).magnitude;
                for (int i = 0; i < validDirection.Count; i++)
                {
                    //Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
                    if (((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude < minDist)
                    {
                        minDistanceIndex = i;
                        //Debug.Log(validDirection[i] + " shortestDist");
                        minDist = ((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude;
                    }
                }
                //direction = validDirection[minDistanceIndex];

                Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
                return validDirection[minDistanceIndex];
            }
            else
            {
                if (validDirection.Count > 0)
                    return validDirection[0];
                else
                {
                    return -currentDir;
                }
            }
        }
        

    }

    Vector2 calcNewAnyDir()
    {
        List<Vector2> validDirection = new List<Vector2>();
        if (valid(Vector2.right))
        {
            validDirection.Add(Vector2.right);
        }
        if (valid(Vector2.up))
        {
            validDirection.Add(Vector2.up);
        }
        if (valid(Vector2.down))
        {
            validDirection.Add(Vector2.down);
        }
        if (valid(Vector2.left))
        {
            validDirection.Add(Vector2.left);
        }



        if (validDirection.Count > 1)
        {
            Debug.Log(validDirection.Count);
            Vector2 pos = transform.position;
            int minDistanceIndex = 0;
            float minDist = ((Vector2)transform.position - targetPoint).magnitude;
            for (int i = 0; i < validDirection.Count; i++)
            {
                //Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
                if (((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude < minDist)
                {
                    minDistanceIndex = i;
                    //Debug.Log(validDirection[i] + " shortestDist");
                    minDist = ((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude;
                }
            }
            //direction = validDirection[minDistanceIndex];

            Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
            return validDirection[minDistanceIndex];
        }
        else
        {
            if (validDirection.Count > 0)
                return validDirection[0];
            else
            {
                return Vector2.zero;
            }
        }

    }

    
    Vector2 calcNewDirScared()
    {
        List<Vector2> validDirection = new List<Vector2>();
        if (valid(Vector2.up))
        {
            validDirection.Add(Vector2.up);
        }
        if (valid(Vector2.down))
        {
            validDirection.Add(Vector2.down);
        }
        if (valid(Vector2.left))
        {
            validDirection.Add(Vector2.left);
        }
        if (valid(Vector2.right))
        {

            validDirection.Add(Vector2.right);
        }
        for (int i = 0; i < validDirection.Count; i++)
        {
            if (validDirection[i] == -currentDir)
            {
                return validDirection[i];
                // Debug.Log(validDirection[i] + " is opposite direction");
                //validDirection.RemoveAt(i);
            }
        }
        Debug.Log(validDirection.Count);
        if (validDirection.Count > 1)
        {
            Debug.Log(validDirection.Count);
            Vector2 pos = transform.position;
            int minDistanceIndex = 0;
            float minDist = ((Vector2)transform.position - targetPoint).magnitude;
            for (int i = 0; i < validDirection.Count; i++)
            {
                //Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
                if (((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude < minDist)
                {
                    minDistanceIndex = i;
                    //Debug.Log(validDirection[i] + " shortestDist");
                    minDist = ((Vector2)transform.position + validDirection[i] * DirectionCalculationClosenessMult - targetPoint).magnitude;
                }
            }
            //direction = validDirection[minDistanceIndex];

            Debug.DrawRay((pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), pos - (pos + validDirection[minDistanceIndex] * circleCastLength * DirectionCalculationClosenessMult), Color.green, 3f);
            return validDirection[minDistanceIndex];
        }
        else
        {
            if (validDirection.Count > 0)
                return validDirection[0];
            else
            {
                return Vector2.zero;
            }
        }
    }
    Vector2 getRoundedPos()
    {
        Vector2 v;
        v.x = Mathf.RoundToInt(transform.position.x * 2f) / 2f;
        v.y = Mathf.RoundToInt(transform.position.y * 2f) / 2f;
        return v;
    }
    void setAnim(Vector2 xy)
    {
        anim.SetFloat("DirX", xy.x);
        anim.SetFloat("DirY", xy.y);
    }
    void setAnim(bool scared)
    {
        anim.SetBool("Run", scared);
    }
    void setAnimDead(bool dead)
    {
        anim.SetBool("Run_White", dead);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Pacman"))
        {
            Debug.Log(myState);
            if (myState != State.frightened)
            {
                manager.Instance.lostLife();
            }
            else 
            {
                //die
                manager.Instance.eatGhost();
                myState = State.dead;
                gameObject.layer = 11;
                setAnimDead(true);
                turnAroundOnNextTween = true;
                //LeanTween.cancel(gameObject);
               // transform.position = getRoundedPos();
                moving = false;
                //tween(calcNewDirScared());
                
            }

            //SceneManager.LoadScene(0);
        }
        
        
    }
    void respawn()
    {
        setAnimDead(false);
        setAnim(false);
        myState = State.chase;
        gameObject.layer = 9;
    }
}
