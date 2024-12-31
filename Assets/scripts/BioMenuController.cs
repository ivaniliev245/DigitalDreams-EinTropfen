using UnityEngine;
using UnityEngine.SceneManagement;

public class BioMenuController : MonoBehaviour
{
    // Name of the scene to load when continue is pressed
    public string gameSceneName;

    // Method to load the game scene
    public void ContinueToGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
