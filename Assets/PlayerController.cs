
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movmentController;
    public SpriteRenderer sprite;
    public Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movmentController = GetComponent<MovementController>();
        movmentController.lastMovingDirection = "left";
    }

    // Update is called once per frame
    void Update()
    {
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
}
