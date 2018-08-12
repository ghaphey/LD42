using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour {
    

    public void StartPressed()
    {
        SceneManager.LoadScene("Level1");
    }


    public void QuitPressed()
    {
        Application.Quit();
    }
    
}
