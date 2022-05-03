
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movmentController;
    public SpriteRenderer sprite;
    public Animator animator;
    public GameObject startNode;
    public Vector2 startPosition;
    public GameManager gameManager;

    public bool isDead = false;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPosition = new Vector2(5.39f, -0.63f);
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movmentController = GetComponent<MovementController>();
        startNode = movmentController.currentNode;
     
    }
   

    public void setup()
    {
        isDead = false;
        animator.SetBool("dead", false);
        animator.SetBool("moving", false);
        movmentController.currentNode = startNode;
        movmentController.lastMovingDirection = "left";
        transform.position = startPosition;
        sprite.flipX = false;
        movmentController.direction = "left";
        animator.speed = 1;
    }
    public void Stop()
    {
        animator.speed = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if(!gameManager.GameIsRunning)
        {
            if (!isDead)
            {
            animator.speed = 0;
            }
            return;
        }
        animator.speed = 1;
        animator.SetBool("moving" ,true );
      if (Input.GetKey(KeyCode.LeftArrow))
        {
            movmentController.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movmentController.SetDirection("right");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movmentController.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movmentController.SetDirection("down");
        }
        bool flipX = false;
        bool flipY = false;

        if (movmentController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction",0);
        }
        else if (movmentController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        else if (movmentController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movmentController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        sprite.flipX = flipX;

    }

    public void Death()
    {
        isDead = true;
        animator.SetBool("moving", false);
        animator.speed = 1;
        animator.SetBool("dead", true);
    }
}
