using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tweenMove : MonoBehaviour
{
    public float tweenTime;
    public float circleCastWidth = .2f;
    public float circleCastLength = .5f;
    public float tweenResolution = .5f;
    public LayerMask ValidCheckCast;
    private Animator anim;
    Vector2 startPos;

    Vector2 currentDir;
    Vector2 lastInput;
    Collider2D mycol;

    bool moving = false;
    bool cantTeleport = false;
    manager manage;
    // Start is called before the first frame update
    void Start()
    {
        gameEvents.current.onLifeLost += lifeLost;
        anim = GetComponent<Animator>();
        mycol = GetComponent<Collider2D>();
        setAnim(Vector2.right);
        manage = manager.Instance;
        startPos = getRoundedPos();

    }
    public Vector2 getDirection()
    {
        return currentDir;
    }
    // Update is called once per frame
    void Update()
    {
        
        testInput();
        if(moving == false && lastInput!= Vector2.zero)
        {
            endTween();
        }
    }
    void tween(Vector2 dir)
    {
        setAnim(dir);
        LeanTween.move(gameObject, getRoundedPos() + dir , tweenTime).setOnComplete(endTween);
        
        moving = true;
    }
    void endTween()
    {
        //Debug.Log("HE<EMDWEMDLE");
        transform.position = getRoundedPos();
       //Debug.Log(transform.position + "    " + getRoundedPos());
        if (lastInput!= Vector2.zero && valid(lastInput))
        {
            tween(lastInput);
            currentDir = lastInput;
            lastInput = Vector2.zero;
            moving = true;
            AudioManager.Instance.play("eat");
        }
        else if (valid(currentDir))
        {
            tween(currentDir);
            
            moving = true;
            AudioManager.Instance.play("eat");
        }
        else
        {
            moving = false;
            
        }
    }
    public void lifeLost()
    {
        manage.lostLife();
        if(manage.Lives > 0)
        {
            LeanTween.cancel(this.gameObject);
            transform.position = getRoundedPos();
            transform.position = startPos;
            lastInput = Vector2.zero;
            moving = false;
        }
        else
        {
            setAnim(Vector2.zero);
            anim.SetBool("Dead", true);
        }
        

    }
    bool valid(Vector2 dir)
    {
        Vector2 pos = transform.position;
        Debug.DrawLine(pos + dir * circleCastLength, pos);
        Debug.DrawLine((pos + dir * circleCastLength + Vector2.Perpendicular(dir) * circleCastWidth), pos + Vector2.Perpendicular(dir) * circleCastWidth);
        Debug.DrawLine(pos + dir * circleCastLength - Vector2.Perpendicular(dir) * circleCastWidth, pos - Vector2.Perpendicular(dir) * circleCastWidth);

        // Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir*circleCastLength, pos,ValidCheckCast);
        RaycastHit2D hit1 = Physics2D.Linecast(pos + dir * circleCastLength + Vector2.Perpendicular(dir)*circleCastWidth, pos + Vector2.Perpendicular(dir) * circleCastWidth,ValidCheckCast);
        RaycastHit2D hit2 = Physics2D.Linecast(pos + dir * circleCastLength - Vector2.Perpendicular(dir) * circleCastWidth, pos - Vector2.Perpendicular(dir) * circleCastWidth,ValidCheckCast);
        //RaycastHit2D hitC = Physics2D.CircleCast()
        //Vector2.r
        if(hit.collider == hit1.collider && hit1.collider == hit2.collider)
        {
            return hit.collider == mycol;
        }
        return false;
        
       
        //RaycastHit2D hit1 = Physics2D.Linecast(pos + dir, pos);
        //return true;
    }
    void testInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            lastInput = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("pacdot"))
        {

            manage.eatPacdot();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("PowerDot"))
        {
            gameEvents.current.powerDot();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Fruit"))
        {
            int s = collision.transform.GetComponent<fruit>().value;
            manage.addScore(s);
            Destroy(collision.gameObject);
            //manage.addScore(collision.gameObject.GetComponents<fruit>().v)
        }

        if (collision.CompareTag("teleport")&&!cantTeleport)
        {
            LeanTween.cancel(this.gameObject);
            if (transform.position.x < 5)
            {
                StartCoroutine(goInvisible(.15f));
                //LeanTween.cancel(gameObject);
               
                transform.position = new Vector2(27.5f, 14.5f);
                setAnim(Vector2.left);
                tween(Vector2.left);
            }
            else
            {
                StartCoroutine(goInvisible(.15f));
                
                transform.position = new Vector2(.5f, 14.5f);
                setAnim(Vector2.right);
                tween(Vector2.right);
            }
        }
    }//pacdot and gate collide
    public IEnumerator goInvisible(float t)
    {
        cantTeleport = true;
        moving = true;
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(t);
        GetComponent<SpriteRenderer>().enabled = true;
        moving = false;
        cantTeleport = false;


    }

    /* void OnDrawGizmosSelected()
     {
         // Draw a yellow sphere at the transform's position
         Gizmos.color = Color.yellow;
         Gizmos.DrawWireSphere(transform.position + (Vector3)currentDir*circleCastLength, circleCastWidth);
     }
    */
}
