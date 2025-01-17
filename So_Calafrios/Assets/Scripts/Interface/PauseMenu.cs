﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Class that interact with the buttons pressed by the player
/// in the pause menu.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerInput player;
    [SerializeField] private GameObject fadeCanvas;
    [SerializeField] private CanvasGroup image;
    [SerializeField] private float transitionSpeed;
    [SerializeField] private float transitionTimeSpeed;
    [SerializeField] private int menuSceneNumber;
    [SerializeField] private static bool gameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI = default;
    [SerializeField] private GameObject options = default;
    [SerializeField] private GameObject helpMenu = default;
    [SerializeField] private PauseManager pauseManager = default;

    /// <summary>
    /// Private method which stops the game and permits the free
    /// movement of the mouse.
    /// </summary>
    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
        AudioListener.pause = true;
        pauseManager.TogglePause();
        player.inGameInputs = false;
    }

    /// <summary>
    /// Public method which resume the game and locks the mouse movement.
    /// </summary>
    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
        AudioListener.pause = false;
        pauseManager.TogglePause();
        player.inGameInputs = true;
    }

    /// <summary>
    /// Public method to returns to the menu scene.
    /// </summary>
    public void LoadMenu()
    {
        player.enabled = false;
        fadeCanvas.SetActive(true);
        StartCoroutine(Transition(menuSceneNumber));
    }

    /// <summary>
    /// Public method to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Public method in reaction to pressing the Escape button.
    /// </summary>
    public void PauseKey()
    {
        if (!gameIsPaused)
        {
            Pause();
        }
        else if(gameIsPaused && options.activeSelf)
        {
            options.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
        else if(gameIsPaused && helpMenu.activeSelf)
        {
            helpMenu.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Resume();
        }
    }

    /// <summary>
    /// Private Coroutine that does the fade in and loads the scene.
    /// </summary>
    /// <param name="sceneNumber">Number of the scene to load.</param>
    private IEnumerator Transition(int sceneNumber)
    {
        // While the image is not fully visible, then it fades in.
        while(image.alpha != 1f)
        {
            image.alpha += transitionSpeed;
            AudioListener.volume -= transitionSpeed;

            if(image.alpha > 1f)
            {
                image.alpha = 1f;
            }
            if(AudioListener.volume < 0f)
            {
                AudioListener.volume = 0f;
            }

            yield return new WaitForSeconds(transitionTimeSpeed);
        }

        gameIsPaused = false;
        AudioListener.pause = false;
        SceneManager.LoadScene(sceneNumber);
    }
}
