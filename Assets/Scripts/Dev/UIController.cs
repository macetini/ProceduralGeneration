using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject HelpPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelpPanel();
        }
    }

    public void ToggleHelpPanel()
    {
        if (HelpPanel != null)
        {
            HelpPanel.SetActive(!HelpPanel.activeSelf);
        }
    }
}
