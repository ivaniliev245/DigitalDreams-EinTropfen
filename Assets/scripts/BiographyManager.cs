using UnityEngine;
using UnityEngine.UI;

public class BiographyManager : MonoBehaviour
{
    public GameObject[] bioPanels; // Array to hold biography panels
    public Button[] buttons; // Array to hold buttons

    private int currentPanelIndex = 0; // Index of the currently open panel

    void Start()
    {
        // Ensure all panels are inactive at start
        foreach (GameObject panel in bioPanels)
        {
            panel.SetActive(false);
        }

        // Open the first biography panel by default
        OpenPanel(0);

        // Add listeners to buttons
        buttons[0].onClick.AddListener(() => OpenPanel(0));
        buttons[1].onClick.AddListener(() => OpenPanel(1));
        buttons[2].onClick.AddListener(() => OpenPanel(2));
        buttons[3].onClick.AddListener(() => OpenPanel(3));
        buttons[4].onClick.AddListener(() => OpenPanel(4));
        buttons[5].onClick.AddListener(() => OpenPanel(5));
    }

    void OpenPanel(int index)
    {
        // Close the currently open panel
        bioPanels[currentPanelIndex].SetActive(false);

        // Open the new panel
        bioPanels[index].SetActive(true);

        // Update the current panel index
        currentPanelIndex = index;
    }
}
