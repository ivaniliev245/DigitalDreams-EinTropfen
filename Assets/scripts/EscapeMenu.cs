using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class EscapeMenu : MonoBehaviour
{
    public GameObject escapeMenuUI; // The UI panel that holds the escape menu
    public Slider audioSlider;
    public Button exitButton;
    public Button mainMenuButton;
    public Button returnButton;
    public Button escapeMenuButton;

    public List<MonoBehaviour> scriptsToDisable; // List of scripts to disable

    private bool isPaused = false;
    private List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>(); // List to keep track of disabled scripts

    void Start()
    {
        // Initially make the escape menu invisible but functional (or active in the scene)
        escapeMenuUI.SetActive(false);

        // Add listeners for buttons and slider
        exitButton.onClick.AddListener(ExitGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        returnButton.onClick.AddListener(Resume);
        escapeMenuButton.onClick.AddListener(ToggleEscapeMenu);
        audioSlider.onValueChanged.AddListener(AdjustAudio);
    }

    void Update()
    {
        // Check if the ESC key is pressed and toggle the escape menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC key pressed"); // Log for debugging
            ToggleEscapeMenu();
        }
    }

    void ToggleEscapeMenu()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Resume()
    {
        escapeMenuUI.SetActive(false); // Hide the escape menu
        escapeMenuButton.gameObject.SetActive(true); // Show escape menu button
        Time.timeScale = 1f; // Resume game
        isPaused = false;

        // Enable previously disabled scripts
        foreach (var script in disabledScripts)
        {
            script.enabled = true;
        }
        disabledScripts.Clear();
    }

    void Pause()
    {
        escapeMenuUI.SetActive(true); // Show the escape menu
        escapeMenuButton.gameObject.SetActive(false); // Hide escape menu button
        Time.timeScale = 0f; // Pause game
        isPaused = true;

        // Disable specified scripts on all game objects
        foreach (var scriptType in scriptsToDisable)
        {
            var scripts = UnityEngine.Object.FindObjectsByType(scriptType.GetType(), FindObjectsSortMode.None) as MonoBehaviour[];
            foreach (var script in scripts)
            {
                if (script.enabled)
                {
                    script.enabled = false;
                    disabledScripts.Add(script);
                }
            }
        }
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
        // If running in the Unity Editor, stop the play mode
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running in a standalone build, exit the application
        Application.Quit();
        #endif
    }

    void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Ensure game is not paused
        SceneManager.LoadScene("MainMenu"); // Load Main Menu scene
    }

    void AdjustAudio(float volume)
    {
        AudioListener.volume = volume; // Adjust audio volume
    }
}
