using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool isPaused;


    private void Awake()
    {
        isPaused = true;
    }

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
        panel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        panel.SetActive(false);
        Time.timeScale = 1;
    }
}
