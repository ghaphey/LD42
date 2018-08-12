using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider volume;
    // Update is called once per frame
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
                Time.timeScale = 0f;
            }
        }
    }

    public void ResumePressed()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenuPressed()
    {
        print("NOT IMPLEMENTED");
    }

    public void QuitPressed()
    {
        Application.Quit();
    }

    public void AdjustVolume()
    {
        AudioListener.volume = volume.value;
    }
}
