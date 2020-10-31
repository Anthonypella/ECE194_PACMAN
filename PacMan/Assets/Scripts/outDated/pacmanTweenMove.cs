using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacmanTweenMove : MonoBehaviour
{
    public float speed;


    public float circleCastWidth;
    public LayerMask pacman;

    Vector2 direction;
    Animator anim;
    //Stack<Vector2> inputs;
    Vector2 lastInput;
    bool moving = false;
    // Start is called before the first frame update
    void Start()
    {
        direction = Vector2.right;
        anim = GetComponent<Animator>();
        //setTween();
    }

    // Update is called once per frame
    void Update()
    {
        testInput();


        if (!moving)
        {
            if (valid(lastInput))
            {
                direction = lastInput;
                setTween();
                setAnim(direction);
                //lastInput = Vector2.zero;
                
            }
            else if(valid(direction))
            {
                setAnim(direction);
                setTween();
            }
            else
            {
                isStopped();
            }
            
        }

    }
    void testInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            lastInput = Vector2.up;
                  
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) )
        {
            lastInput = Vector2.down;
           

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            lastInput = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            lastInput = Vector2.left;
        }
    }
    void isStopped()
    {
        transform.position = getRoundedPos();
        moving = false;
    }
    Vector2 getRoundedPos()
    {
        Vector2 v;
        v.x = Mathf.RoundToInt(transform.position.x * 2f) / 2f;
        v.y = Mathf.RoundToInt(transform.position.y * 2f) / 2f;
        return v;
    }
    void setTween()
    {
        //Debug.Log()
        if (valid(direction))
        {
            LeanTween.move(gameObject, (Vector2)transform.position + direction, speed).setOnComplete(endTween);
            moving = true;
        }
            
        else
            isStopped();
        
    }
    
        
    
    void endTween()
    {
        moving = false;
    }

    bool valid(Vector2 dir)
    {
        Vector2 pos = transform.position;

        if (Physics2D.CircleCast(pos, circleCastWidth, pos + dir, 1, pacman))
        {
            return false;
        }
        //RaycastHit2D hit1 = Physics2D.Linecast(pos + dir, pos);
        return true;
    }
    void setAnim(float x, float y)
    {
        anim.SetFloat("DirX", x);
        anim.SetFloat("DirY", y);
    }
    void setAnim(Vector2 xy)
    {
        anim.SetFloat("DirX", xy.x);
        anim.SetFloat("DirY", xy.y);
    }
}
