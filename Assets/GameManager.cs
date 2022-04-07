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

    // Start is called before the first frame update
    void Awake()
    {
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        pacman = GameObject.Find("Player");
        score = 0;
        currentMunch = 0;
        ramadanSong.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        addToScore(10);
    }
}
