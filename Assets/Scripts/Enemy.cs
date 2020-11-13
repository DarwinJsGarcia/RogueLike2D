/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
    public AudioClip enemyAttack1, enemyAttack2;
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;


    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override bool AttemptMove(int xDir, int yDir)
    {
        if(skipMove)
        {
            skipMove = false;
            return false;
        }
        bool canMove = base.AttemptMove(xDir, yDir);
        skipMove = true;
        return canMove;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int  yDir = 0;
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove(xDir, yDir);
    }

    protected override void OnCantMove(GameObject go)
    {
        Player hitPlayer = go.GetComponent<Player>();
        if(hitPlayer != null)
        {
            hitPlayer.LoseFood(playerDamage);
            animator.SetTrigger("enemyAttack");
            SoundManager.instance.RandomizeSfx(enemyAttack1,enemyAttack2);
        }
    }
}
