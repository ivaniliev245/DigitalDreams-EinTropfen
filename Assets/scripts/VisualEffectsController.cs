using System.Collections; // Add this line
using UnityEngine;

public class VisualEffectsController : MonoBehaviour
{
    public Color calmColor = new Color(1f, 0.8f, 0.6f); // Warmer colors for calm effect
    public Color initialColor = Color.white;
    private Camera mainCamera;
    private float transitionSpeed = 0.05f;
    private bool isMeditationActive = false;

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.backgroundColor = initialColor;
    }

    public void TriggerCalmEffect()
    {
        isMeditationActive = true;
        StartCoroutine(ChangeBackgroundColor(calmColor));
    }

    private IEnumerator ChangeBackgroundColor(Color targetColor)
    {
        while (isMeditationActive && mainCamera.backgroundColor != targetColor)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, targetColor, transitionSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
