using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string sceneName; // Assign the scene name in the inspector
    [SerializeField] private float delayInSeconds = 5f; // Adjustable delay

    private void Start()
    {
        Invoke("SwitchScene", delayInSeconds);
    }

    private void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not assigned in the inspector.");
        }
    }
}
