using System.Collections;
/*using System.Collections.Generic;*/
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;


    private float movementSpeed;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        movementSpeed = 1f / moveTime;
    }

    //.. hacer las corrutinas
    protected IEnumerator SmoothMovement(Vector2 end)
    {
        float remainingDistance = Vector2.Distance(rb2D.position, end);
        while(remainingDistance > float.Epsilon)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb2D.position, end, movementSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            remainingDistance = Vector2.Distance(rb2D.position, end);
            yield return null;
        }
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)//mover al jugador
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2 (xDir, yDir); //(0,0) + (1,0) = (1,0)
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        if(hit.transform == null)
        {
            //Hacer el movimiento con la Corrutina.
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected abstract void OnCantMove(GameObject go);

    protected virtual bool AttemptMove(int xDir, int yDir)
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if(!canMove)
        {
            OnCantMove(hit.transform.gameObject);
        }
        return canMove;      
    }
}


