using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    private Text scoreText;

    private int shelfScore = 0;
    private int deliveryScore = 0;
    private int PickupScore = 0;
    private int incineratorScore = 0;
    private int timer = 0;

    private int totalScore = 0;
    private float pps = 0f;

	// Use this for initialization
	void Start () {
        scoreText = GetComponent<Text>();
        scoreText.text = "0";
	}

    private void Update()
    {
        if (Time.time > timer)
        {
            timer++;
            UpdateMainScore();
            print("update");
        }
    }

    private void UpdateMainScore()
    {
        totalScore = shelfScore + deliveryScore + PickupScore + incineratorScore;
        scoreText.text = totalScore.ToString();
        if (totalScore != 0)
            pps = totalScore / timer;
    }

    public void AddShelfScore (int points)
    {
        shelfScore += points;
    }

    public void AddDeliverScore (int points)
    {
        deliveryScore += points;
    }

    public void AddPickupScore(int points)
    {
        PickupScore += points;
    }

    public void AddIncineratorScore(int points)
    {
        incineratorScore += points;
    }
}
