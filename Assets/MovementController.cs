using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovementController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject currentNode;
    public float speed = 2.5f;
    public string direction = "";
    public string lastMovingDirection = "";
    public bool canWarp = true;

    public bool isGhost = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        NodeController currentNodeController = currentNode.GetComponent<NodeController>();
        transform.position = Vector2.MoveTowards(transform.position,currentNode.transform.position,speed * Time.deltaTime);
        bool reverseDirection = false;
        if (
           (direction == "left" && lastMovingDirection == "right") 
           || (direction == "right" && lastMovingDirection == "left") 
           || (direction == "up" && lastMovingDirection == "down") 
           || (direction == "down" && lastMovingDirection == "up")
           )
        {
            reverseDirection = true;
        }

        // Figure out if we're at the center of our current node
        if ((transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y) || reverseDirection)
        {
            if (isGhost)
            {
                GetComponent<EnemyController>().ReachedCenterOfNode(currentNodeController);
            }

            
            if (currentNodeController.isWarpLeftNode && canWarp) //if we reached to the left , warp to the right warp
            {
                currentNode = gameManager.rightWarpNode;
                direction = "left";
                lastMovingDirection = "left";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else if (currentNodeController.isWarpRightNode && canWarp)  //if we reached to the right , warp to the left warp
            {
                currentNode = gameManager.leftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else  //other nodes
            {
                // If we are not a ghost that is respawning, and we are on the start node, and we are trying to move down, stop
                if(currentNodeController.isGhostStartingNode && direction == "down" 
                    && (!isGhost || GetComponent<EnemyController>().ghostNodeState != EnemyController.GhostNodeStatesEnum.respawning))
                {
                    direction = lastMovingDirection;
                }

                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                if (newNode != null)
                {
                    currentNode = newNode;
                    lastMovingDirection = direction;

                }

                else
                {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);
                    if (newNode != null)
                    {
                        currentNode = newNode;
                    }
                }
            }
           
        }
        else
        {
            canWarp = true;
        }
    }

    public void SetDirection (string newDirection)
    {
        direction = newDirection;
    }
}
