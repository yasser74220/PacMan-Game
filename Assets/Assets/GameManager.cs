using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pacman;

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource ramadanSong;
    public AudioSource munch1;
    public AudioSource munch2;
    public int currentMunch;

    public int score;
    public Text scoreText;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollected;

    public bool hadDeathOnThisLevel=false;
    public bool GameIsRunning;
    public bool newGame;
    public bool clearedLevel;
    public AudioSource startGameAudio;
    public int lives;
    public int currentlevel;

    public List<NodeController> nodeControllers = new List<NodeController>();
    

    public enum GhostMode
    {
        chase,scatter
    }


    public GhostMode currentGosteMode;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(setup());

    }

    void Awake()

    {
        newGame = true;
        clearedLevel = false;
        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("Player");

        
    }

    public IEnumerator setup()
    {
        if(clearedLevel)
        {
            yield return new WaitForSeconds(0.1f);
        }
        pelletsCollected = 0;
        currentGosteMode = GhostMode.scatter;
        GameIsRunning = false;
        currentMunch = 0;

        float waittimer = 1f;

        if (clearedLevel || newGame)
        {
            waittimer = 4f;
            //pellete will respone when pacman cleares level or start again 
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].Responepellet();

            }

        }
        if(newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "score" + score.ToString();
            lives = 3;
            currentlevel = 1;
        }

      
        pacman.GetComponent<PlayerController>().setup();

        redGhostController.setup();
        pinkGhostController.setup();
        blueGhostController.setup();
        orangeGhostController.setup();
        newGame = false;
        clearedLevel = false;
        yield return new WaitForSeconds(waittimer);
        StartGame();

    }
    void StartGame()
    {
        GameIsRunning = true;
        ramadanSong.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void getpelletfromNodeControler(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }
    public void addToScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score.ToString();
    }

    public void collectedPellet(NodeController nodeController)
    {
        if(currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if(currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletsCollected++;
        int requiredBluePellets = 0;
        int requiredOrangePellets = 0;
        if(hadDeathOnThisLevel )
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }
        if(pelletsCollected >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefor)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }
        if (pelletsCollected >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefor)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }


        addToScore(10);
    }
}
