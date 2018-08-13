using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIController : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject tutorialPage;
    [SerializeField] private Text scoreText;
    [SerializeField] private Slider volume;
    [SerializeField] private Slider numTrucks;
    [SerializeField] private Text numTrucksText;
    [SerializeField] private ScoreBoard score;

    private int trucksRemaining = 20;

    private void Start()
    {
        Time.timeScale = 0f;
        if (!tutorialPage.activeSelf)
            tutorialPage.SetActive(true);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pauseMenu.SetActive(true);
                pauseMenu.GetComponent<RectTransform>().SetAsLastSibling();
                Time.timeScale = 0f;
            }
        }
        if (trucksRemaining <= 0)
        {
            Time.timeScale = 0f;
            EndGame();
        }
        else if ( trucksRemaining < 5)
        {
            numTrucksText.color = Color.red;
        }
    }

    private void EndGame()
    {
        scoreText.text = score.OutputScoreScreenText();
        scoreText.transform.parent.gameObject.SetActive(true);
        scoreText.transform.parent.SetAsLastSibling();
    }

    public void StartPressed()
    {
        numTrucksText.text = trucksRemaining.ToString();
        Time.timeScale = 1f;
        tutorialPage.SetActive(false);
    }

    public void TruckDepart()
    {
        numTrucksText.color = Color.white;
        trucksRemaining--;
        numTrucksText.text = trucksRemaining.ToString();
    }

    public int TrucksRemain()

    {
        return trucksRemaining;
    }

    public void ResumePressed()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenuPressed()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }

    public void QuitPressed()
    {
        Application.Quit();
    }

    public void AdjustVolume()
    {
        AudioListener.volume = volume.value;
    }

    public void AdjustNumTrucks()
    {
        trucksRemaining = Mathf.FloorToInt(numTrucks.value);
        numTrucks.GetComponentInChildren<Text>().text = trucksRemaining.ToString();
    }
}
