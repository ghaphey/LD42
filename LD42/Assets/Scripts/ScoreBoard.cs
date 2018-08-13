using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    private Text scoreText;

    private int shelfScore = 0;
    private int deliveryScore = 0;
    private int pickupScore = 0;
    private int incineratorScore = 0;
    private int timer = 0;

    private int totalScore = 0;
    private float pps = 0f;
    private int numDeliveries = 0;
    private int numPickups = 0;

	// Use this for initialization
	void Start () {
        scoreText = GetComponent<Text>();
        scoreText.text = "0";
	}

    // ONLY UPDATE SCORE ONCE PER SECOND
    private void Update()
    {
        if (Time.time > timer)
        {
            timer++;
            UpdateMainScore();
        }
    }

    private void UpdateMainScore()
    {
        totalScore = shelfScore + deliveryScore + pickupScore + incineratorScore;
        scoreText.text = totalScore.ToString();
    }

    public void AddShelfScore (int points)
    {
        shelfScore += points;
    }

    public void AddDeliverScore (int points)
    {
        deliveryScore += points;
        numDeliveries++;
    }

    public void AddPickupScore(int points)
    {
        pickupScore += points;
        numPickups++;
    }

    public void AddIncineratorScore(int points)
    {
        incineratorScore += points;
    }

    // CREATES A STRING FOR THE FINAL SCORE SCREEN
    public string OutputScoreScreenText()
    {
        totalScore = shelfScore + deliveryScore + pickupScore + incineratorScore;
        pps = totalScore / timer;
        return numPickups.ToString() + " / " + pickupScore.ToString() + "\n\n"
               + numDeliveries.ToString() + " / " + deliveryScore.ToString() + "\n\n"
               + shelfScore.ToString() + "\n\n"
               + incineratorScore.ToString() + "\n\n"
               + totalScore + "\n\n"
               + timer.ToString() + " seconds\n\n"
               + pps.ToString();
    }
}
