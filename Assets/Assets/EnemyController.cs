using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodeStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum startghostNodeState;
    public GhostNodeStatesEnum respawnState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;
    
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public MovementController movementController;

    public GameObject startingNode;

    public bool readyToLeaveHome = false;

    public GameManager gameManager;

    public bool testRespawn = false;
    public bool isFrightened = false;
    public GameObject[] scatterNodes;
    public int scatterNodesindex;
    public bool leftHomeBefor = false;

    // Start is called before the first frame update
    void Awake()
    {
        
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (ghostType == GhostType.red)
        {
            startghostNodeState = GhostNodeStatesEnum.startNode;
            startingNode = ghostNodeStart;
            respawnState = GhostNodeStatesEnum.centerNode;
            
        }
        else if(ghostType == GhostType.pink)
        {
            startghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if(ghostType == GhostType.blue)
        {
            startghostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodeStatesEnum.leftNode;
        }
        else if(ghostType == GhostType.orange)
        {
            startghostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodeStatesEnum.rightNode;
        }
        
        

    }

  

    public void setup()
    {
        
        ghostNodeState = startghostNodeState;
        //reset ghostes back to their position
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
        //set their scatter node index to 0
        scatterNodesindex = 0;
        //set frightened
        isFrightened = false;

       //set ready to leave home to false if blue or pink
        if(ghostType==GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefor = true;
        }
        else if (ghostType==GhostType.pink)
        {
            readyToLeaveHome = true;

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameManager.GameIsRunning)
        {
            return;
        }
        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        if(movementController.currentNode.GetComponent<NodeController>().isSideNodes)
        {
            movementController.SetSpeed(2);
        }
        else
        {
            movementController.SetSpeed(3);
        }
       
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            leftHomeBefor = true;
            //if we reach the scatter mode,add one to our scatter node index
            if (gameManager.currentGosteMode == GameManager.GhostMode.scatter)
            {
                DeterminGhosteScatterModeDirection();

            }
            //frightened mode
            else if(isFrightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            // chase mode
            else
            {
                // Determine next node to go to
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
                else if (ghostType==GhostType.pink)
                {
                    DeterminePinkGhostDirection();
                }
                else if (ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDirection();
                }
                else if (ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDirection();
                }
            }
        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";

            // We have reached our start node, move to the center node
            if(transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            // We have reached our center node, either finish respawn, or move to the left/right node
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            // If our respawn state is either the left or right node, and we got to that node, leave home again
            else if(
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                || (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            // We are in the gameboard still, locate our start node
            else
            {
                // Determine quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            
            movementController.SetDirection(direction);
        }
        else
        {
            if (readyToLeaveHome)
            {
                // If we are in the left home node, move to the center
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                // If we are in the right home node, move to the center
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                // If we are in the center node, move to the start node
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                // If we are in the start node, start moving around in the game
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }
    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();
        if(nodeController.canMoveDown && movementController.lastMovingDirection != "up")
        {
            possibleDirections.Add("down");
        }
        if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
        {
            possibleDirections.Add("up");
        }
        if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
        {
            possibleDirections.Add("right");
        }
        if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
        {
            possibleDirections.Add("left");
        }
        string direction = "";
        int randomDirecrionIndex = UnityEngine.Random.Range(0, possibleDirections.Count - 1);
        direction = possibleDirections[randomDirecrionIndex];
        return direction;
    }
    void DeterminGhosteScatterModeDirection()
    {
        if (transform.position.x == scatterNodes[scatterNodesindex].transform.position.x
            || transform.position.y == scatterNodes[scatterNodesindex].transform.position.y)
        {

            scatterNodesindex++;
            if (scatterNodesindex == scatterNodes.Length - 1)
            {
                scatterNodesindex = 0;
            }

        }
        string direction = GetClosestDirection(scatterNodes[scatterNodesindex].transform.position);
        movementController.SetDirection(direction);

    }
    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    void DeterminePinkGhostDirection()
    {
        string pacmandirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distnaceBetweenNodes = 0.3f;
        Vector2 target = gameManager.pacman.transform.position;
        if(pacmandirection == "left")
        {
            target.x = target.x - (distnaceBetweenNodes * 2);
        }
        else if(pacmandirection == "right")
        {
            target.x = target.x + (distnaceBetweenNodes * 2);

        }
        else if (pacmandirection == "up")
        {
            target.y = target.x + (distnaceBetweenNodes * 2);

        }
        else if (pacmandirection == "down")
        {
            target.y = target.x - (distnaceBetweenNodes * 2);

        }
        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }

    void DetermineBlueGhostDirection()
    {
        string pacmandirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distnaceBetweenNodes = 0.3f;
        Vector2 target = gameManager.pacman.transform.position;
        if (pacmandirection == "left")
        {
            target.x = target.x - (distnaceBetweenNodes * 2);
        }
        else if (pacmandirection == "right")
        {
            target.x = target.x + (distnaceBetweenNodes * 2);

        }
        else if (pacmandirection == "up")
        {
            target.y = target.x + (distnaceBetweenNodes * 2);

        }
        else if (pacmandirection == "down")
        {
            target.y = target.x - (distnaceBetweenNodes * 2);

        }
        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;
        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
    }

    void DetermineOrangeGhostDirection()
    {
        float distnaceBetweenNodes = 0.3f;
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        if(distance<0)
        {
            distance *= -1;
        }
        // if we are within 8 nodes of pacman , chasing him using red logic
        if(distance <= distnaceBetweenNodes * 8)
        {
            DetermineRedGhostDirection();
        }else
        {
            //scatter mode
            DeterminGhosteScatterModeDirection();
        }

    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        // If we can move up and we aren't reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            // Get the node above us
            GameObject nodeUp = nodeController.nodeUp;
            // Get the distance between the node above us (the ghost), and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            // Get the node beneath us
            GameObject nodeDown = nodeController.nodeDown;
            // Get the distance between the node beneath us (the ghost), and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            // Get the node that is on our left
            GameObject nodeLeft = nodeController.nodeLeft;
            // Get the distance between the node that is on our left, and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            // Get the node that is on our left
            GameObject nodeRight = nodeController.nodeRight;
            // Get the distance between the node that is on our left, and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }
}
