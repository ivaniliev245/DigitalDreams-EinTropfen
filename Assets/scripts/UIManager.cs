using UnityEngine;
using UnityEngine.UI;  // For UI components
using UnityEngine.SceneManagement;  // For scene loading

public class UIManager : MonoBehaviour
{
    public Button playButton;
    public Button exitButton;
    public Button muteButton;
    public Button galleryButton;  // New button for the gallery
    public Slider volumeSlider;
    public AudioSource audioSource;

    private bool isMuted = false;

    void Start()
    {
        // Set the initial volume based on the slider
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        audioSource.volume = volumeSlider.value;

        // Add listeners to buttons and slider
        playButton.onClick.AddListener(PlayGame);
        exitButton.onClick.AddListener(ExitGame);
        muteButton.onClick.AddListener(MuteAudio);
        galleryButton.onClick.AddListener(OpenGallery);  // Add listener for the gallery button
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void PlayGame()
    {
        // Load the scene to start the game (replace "Main_Game" with your actual scene name)
        SceneManager.LoadScene("Main_Game");
    }

    void ExitGame()
    {
        // Exit the application
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Stop play mode in editor
        #endif
    }

    void MuteAudio()
    {
        isMuted = !isMuted;
        audioSource.mute = isMuted;
        muteButton.GetComponentInChildren<Text>().text = isMuted ? "Unmute" : "Mute";  // Change button text
    }

    void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);  // Save volume setting
    }

    void OpenGallery()
    {
        // Load the gallery scene (replace "GalleryScene" with your actual scene name)
        SceneManager.LoadScene("GalleryScene");
    }
}
