/*using System.Collections;
using System.Collections.Generic;*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;


    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    private Animator animator;
    private int food;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected override void Start()
    {
        food  = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    void CheckIfGameOver()
    {
        if(food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    protected override bool AttemptMove(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        bool canMove = base.AttemptMove(xDir,yDir);
        if (canMove) SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
        return canMove;
    }
    
    void Update()
    {
        if(!GameManager.instance.playersTurn || GameManager.instance.doingSetup) return;

        int horizontal;
        int vertical;
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        if(horizontal != 0) vertical = 0;
        if(horizontal!=0 || vertical!=0) AttemptMove(horizontal, vertical);
    }

    protected override void OnCantMove(GameObject go)
    {
        wall hitWall = go.GetComponent<wall>();
        if(hitWall != null)
        {
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");
        }
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseFood(int loss)
    {
        food -= loss;
        foodText.text = "-"+ loss +" Food: " + food;
        animator.SetTrigger("playerHit");
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Exit"))
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }else if(other.CompareTag("Food"))
        {
            food += pointsPerFood;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            foodText.text = "+"+ pointsPerFood +" Food: " + food;
            other.gameObject.SetActive(false);
        }else if(other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            foodText.text = "+"+ pointsPerSoda +" Food: " + food;
            other.gameObject.SetActive(false);
        }
    }
}
