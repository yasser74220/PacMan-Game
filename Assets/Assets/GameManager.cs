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
    public AudioSource death;
    public AudioSource powerPelletSound;

    

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

    public Image BlackBackground;
    public Text  gameOverText;
    public Text LivesText;

    public enum GhostMode
    {
        chase,scatter
    }


    public GhostMode currentGosteMode;

    public int[] ghostModeTimers = new int[] { 7, 20, 7, 20, 5, 20, 5 };
    public int ghostModeTimerIndex;
    public float ghostModeTimer ;
    public bool runningTimer;
    public bool completedTimer;

    public bool isPowerPelletRunning = false;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 5f;
    public int powerPelletMultiplyer = 1;


    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(setup());

    }   

    void Awake()

    {
        newGame = true;
        clearedLevel = false;
        BlackBackground.enabled = false;
        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("Player");

        
    }

    public IEnumerator setup()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        completedTimer = false;
        runningTimer = true;
        gameOverText.enabled = false;
        if(clearedLevel)
        {
            BlackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        BlackBackground.enabled = false;

        pelletsCollected = 0;
        currentGosteMode = GhostMode.scatter;
        GameIsRunning = false;
        currentMunch = 0;

        float waittimer = 1f;

        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets;
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
            SetLives(3);
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
    void SetLives(int newLives)
    {
        lives = newLives;
        LivesText.text = "Lives: " + lives;
    }
    void StartGame()
    {
        GameIsRunning = true;
        ramadanSong.Play();
    }
    void StopGame()
    {
        GameIsRunning = false;
        ramadanSong.Stop();
        powerPelletSound.Stop();
        pacman.GetComponent<PlayerController>().Stop();
        // pacman.GetComponent<PlayerController>().Stop();
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameIsRunning)
        {
            return;
        }

        

        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if(ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex++;
                if (currentGosteMode == GhostMode.chase)
                {
                    currentGosteMode = GhostMode.scatter;
                }
                else
                {
                    currentGosteMode = GhostMode.chase;
                }
                
                if(ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGosteMode = GhostMode.chase;
                }
            }
        }

        if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime;
            if (currentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletSound.Stop();
                powerPelletMultiplyer = 1;
            }
        }
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

    public IEnumerator collectedPellet(NodeController nodeController)
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
        if (pelletsLeft == 3)
        {
            currentlevel++;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(setup());

        }

        if (nodeController.isPowerPellet)
        {
            powerPelletSound.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;
            redGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);
        }
    }

    public IEnumerator PauseGame(float TimeToPause)
    {
        GameIsRunning = false;
        yield return new WaitForSeconds(TimeToPause);
        GameIsRunning = true;
    }
    public void GhostEaten()
    {
        addToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));
    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        StopGame();
        yield return new WaitForSeconds(1);
        redGhostController.SetVisible(false);
        pinkGhostController.SetVisible(false);
        blueGhostController.SetVisible(false);
        orangeGhostController.SetVisible(false);
        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        yield return new WaitForSeconds(3);

        SetLives(lives - 1);
        if (lives == 0)
        {
            newGame = true;
            gameOverText.enabled = true;
            yield return new WaitForSeconds(3);

        }
        StartCoroutine(setup());
    }
}
