using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMove : MonoBehaviour
{

    private Rigidbody2D rbody;
    Vector2 targetPoint;
    Vector2 direction;
    public float circleCastWidth;
    public LayerMask myLayer;
    private Transform Pac;
    public float speed;
    Collider2D myCol;

    
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        direction = Vector2.right;
        Pac = GameObject.Find("pacman").transform;
        targetPoint = Pac.position;
        myCol = GetComponent<Collider2D>();

    }

    private void FixedUpdate()
    {
        if (valid(direction))
        {
            rbody.MovePosition(Vector2.MoveTowards((Vector2)transform.position, (Vector2)transform.position + direction, speed));

        }
    }
    // Update is called once per frame
    void Update()
    {
        
        
    }
    Vector2 calcNewDir()
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
            if (validDirection[i] == -direction)
            {
                validDirection.RemoveAt(i);
            }
        }
        if (validDirection.Count > 1)
        {
            Debug.Log("DifferentDirections");
            int minDistanceIndex = 0;
            float minDist = Vector2.SqrMagnitude((Vector2)transform.position - targetPoint);
            for (int i = 0; i < validDirection.Count; i++)
            {
                if (Vector2.SqrMagnitude((Vector2)transform.position + validDirection[i] - targetPoint) < minDist)
                {
                    minDistanceIndex = i;
                    minDist = Vector2.SqrMagnitude((Vector2)transform.position + validDirection[i] - targetPoint);
                }
            }
            //direction = validDirection[minDistanceIndex];
            return validDirection[minDistanceIndex];
        }
        else
        {
            return validDirection[0];
        }

    }
    bool valid(Vector2 dir)
    {
        
       
            // Cast Line from 'next to Pac-Man' to 'Pac-Man'
            Vector2 pos = transform.position;
            RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
            return (hit.collider == myCol);
        
        /*
        if (Physics2D.CircleCast(pos, circleCastWidth, pos + dir, .5f, myLayer))
        {
            return false;
        }
        */
        //RaycastHit2D hit1 = Physics2D.Linecast(pos + dir, pos);
       
    }
}
