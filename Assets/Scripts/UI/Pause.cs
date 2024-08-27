using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private GameObject testbutton;
    void Update()
    {
        //this is literally for testing on the computers
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        testbutton.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 0;
        testbutton.SetActive(false);
    }
}
