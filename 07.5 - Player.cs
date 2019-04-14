﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour {

    private int lives = 3;
    public Text livesText;

    public int score = 0;
    public Text scoreText;

    public int hp = 6;

    public float atkDmg;
    public float speed;
    public float jumpForce;
    public float groundCheckRadius;

    private float moveValue;
    private float floatValue;
    private int dmgTaken;

    public bool dead = false;
    public bool grounded;

    private bool facingRight;
    private bool canMove = true;
    private bool canCrouch = true;
    private bool sucking = false;
    private bool suckTransform = false;

    private AttackForm form;
    private State currentState;

    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask isGroundLayer;

    public GameObject suckRadius;
    public GameObject suckHitBox;
    public GameObject suckEffect;
    public GameObject spawnPoint;

    public GameObject hpIndex1;
    public GameObject hpIndex2;
    public GameObject hpIndex3;
    public GameObject hpIndex4;
    public GameObject hpIndex5;
    public GameObject hpIndex6;

    public GameObject gameOverMenu;


    Animator anim;

	void Start () {

        tag = "Player";

        if (PlayerPrefs.HasKey("PlayerName"))
            name = PlayerPrefs.GetString("PlayerName");
        else
            name = "Player";

        rb = GetComponent<Rigidbody2D>();
        rb.mass = 1.0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        facingRight = true;

        anim = GetComponent<Animator>();

        currentState = State.Normal;
        form = AttackForm.None;

        scoreText.text = "Score : " + score;
        livesText.text = "Lives : " + lives;

        CheckList();
    }

	void Update () {

        if (groundCheck)
        {
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
        }

        if (canMove)
        {
            moveValue = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveValue * speed, rb.velocity.y); // Move
            Flip(moveValue);

            if (grounded) // Any action when on ground
            {   
                // Jump
                if (Input.GetButtonDown("Jump")) // Spacebar // Input.GetKeyDown(KeyCode.Space)
                {
                    StartCoroutine(Jump());
                }
                anim.SetBool("Float",false);
            }

            if (Input.GetButtonDown("Up"))
            {
                floatValue = Input.GetAxisRaw("Up");
                rb.AddForce(Vector2.up * jumpForce / 1.8f, ForceMode2D.Impulse);
                anim.SetBool("Float", true);
            }

            //if (Input.GetKeyDown(KeyCode.C))
            //    floatValue = 0.0f;
            //    anim.SetFloat("Float", Mathf.Abs(floatValue));

        } // Move Bracket    
        StartCoroutine(SuckCheck());
        CrouchCheck();

        if (anim)
        {
            switch (currentState)
            {
                case State.Normal:
                    anim.SetFloat("Movement", Mathf.Abs(moveValue));
                    break;

                case State.Full:
                    anim.SetFloat("Full Movement", Mathf.Abs(moveValue));
                    break;
            }
        }
    }

   
     IEnumerator Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetBool("Jump", true);
        yield return new WaitForSeconds(1.1f);
        anim.SetBool("Jump", false);
    }

    IEnumerator SuckCheck()
    {
        if (Input.GetMouseButtonDown(1))
        {
            canMove = false;
            canCrouch = false;
            sucking = true;
            anim.SetBool("Suck", true);
            yield return new WaitForSeconds(1);
            suckHitBox.SetActive(true);
            suckRadius.SetActive(true);
            anim.speed = 0.0f;
        }
        if (Input.GetMouseButtonUp(1))
        {
            suckHitBox.SetActive(false);
            suckRadius.SetActive(false);
            anim.speed = 1f;
            canMove = true;
            canCrouch = true;
            sucking = false;
            anim.SetBool("Suck", false);
        }
    }

    void CrouchCheck()
    {
        if (canCrouch)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
              //  rb.velocity = new Vector2(0, rb.velocity.y);  
             //   moveValue = 0.0f;
                canMove = false;
                anim.SetBool("Crouching", true);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                anim.SetBool("Crouching", false);
              
                canMove = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D enemy)
    {
        if (enemy.gameObject.tag == "Enemy")
        {
            BaseEnemy mob = enemy.gameObject.GetComponent<BaseEnemy>();

            if (sucking == true)
            {
                mob.Die();
                currentState = State.Full;
                suckTransform = true;
                anim.SetBool("Suck", false);
                anim.SetBool("Full", true);
            }
            else
            {
                dmgTaken = mob.atkDmg;
                TakeDamage(dmgTaken);
                mob.Die();
            }
            score += mob.pointValue;
            scoreText.text = "Score : " + score;
        }
    }

    public void TakeDamage(int dmgTaken)
    {

        FindObjectOfType<AudioManager>().Play("Hit");
        anim.SetBool("Hit", true);
        hp -= dmgTaken;
        canMove = false;
        switch (hp)
        {
            case 5:
                hpIndex6.SetActive(false);
                break;
            case 4:
                hpIndex6.SetActive(false);
                hpIndex5.SetActive(false);
                break;
            case 3:
                hpIndex6.SetActive(false);
                hpIndex5.SetActive(false);
                hpIndex4.SetActive(false);
                break;
            case 2:
                hpIndex6.SetActive(false);
                hpIndex5.SetActive(false);
                hpIndex4.SetActive(false);
                hpIndex3.SetActive(false);
                break;
            case 1:
                hpIndex6.SetActive(false);
                hpIndex5.SetActive(false);
                hpIndex4.SetActive(false);
                hpIndex3.SetActive(false);
                hpIndex2.SetActive(false);
                break;
            case 0:
                hpIndex6.SetActive(false);
                hpIndex5.SetActive(false);
                hpIndex4.SetActive(false);
                hpIndex3.SetActive(false);
                hpIndex2.SetActive(false);
                hpIndex1.SetActive(false);
                break;
        }

        if (hp > 0)
        {
            StartCoroutine(Hit());
        }
        else
        {
            lives -= 1;
            if (lives == 0)
            {
               Invoke("PlayAgainMenu", 1);
            }
            else
            {
                StartCoroutine(Die());
            }
        }   
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(2);
        anim.SetBool("Hit", false);
        currentState = State.Normal;
        canMove = true;
    }

    IEnumerator Die()
    {

        FindObjectOfType<AudioManager>().Play("Hit");
        anim.SetBool("Dead", true);
        yield return new WaitForSeconds(3);
        transform.position = spawnPoint.transform.position;
        livesText.text = "Lives : " + lives;
        anim.SetBool("Hit", false);
        anim.SetBool("Dead", false);
        hp = 6;
        canMove = true;

        hpIndex1.SetActive(true);
        hpIndex2.SetActive(true);
        hpIndex3.SetActive(true);
        hpIndex4.SetActive(true);
        hpIndex5.SetActive(true);
        hpIndex6.SetActive(true);
        
    }

    private void Flip(float moveValue)
    {
        if (moveValue > 0 && !facingRight || moveValue < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 playerScale = transform.localScale;
            playerScale.x *= -1;
            transform.localScale = playerScale;
        }
    }

    public enum State
    {
        Normal,
        Full
    }

    public enum AttackForm
    {
        None,
        Fire,
        Spark,
        Beam
    }

    public void CheckList()
    {
        if (speed < 0 || speed > 5.0f)
        {
            speed = 5.0f;
            Debug.LogWarning("Speed not set on " + name + ". Defaulting to " + speed);
        }

        if (jumpForce <= 0 || jumpForce > 10.0f)
        {
            jumpForce = 10.0f;
            Debug.LogWarning("JumpForce not set on " + name + ". Defaulting to " + jumpForce);
        }

        if (!groundCheck)
        {
            groundCheck = GameObject.Find("GroundCheck").GetComponent<Transform>(); // In hierarchy

        }
    }

    public void PlayAgainMenu()
    {
        gameOverMenu.SetActive(true);


    }

}
/* Ignore This
    * 	void Update () {
       if (canMove)
       {
           float moveValue = Input.GetAxisRaw("Horizontal");

           if (groundCheck)
           {
               grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
           }

           if (grounded) // Any action when on ground
           {
               if (Input.GetButtonDown("Jump")) // Spacebar // Input.GetKeyDown(KeyCode.Space)
               {
                   rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                   anim.SetBool("Jump", true);
               }
               if (anim)
               {
                   switch (currentState)
                   {
                       case State.Normal:
                           anim.SetFloat("Movement", Mathf.Abs(moveValue));
                           break;

                       case State.Full:
                           anim.SetFloat("Full Movement", Mathf.Abs(moveValue));
                           break;
                   }
               }
               anim.SetBool("Jump", false);
           }
           rb.velocity = new Vector2(moveValue * speed, rb.velocity.y); // Move
           Flip(moveValue);

       } // Move Bracket    
       StartCoroutine(SuckCheck());
       CrouchCheck();
   }
    */
