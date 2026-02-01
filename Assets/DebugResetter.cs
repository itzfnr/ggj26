using UnityEngine;
using UnityEngine.UI; // Required for the Button component
using UnityEngine.SceneManagement; // Required for scene loading

public class DebugResetter : MonoBehaviour
{
    private Button button;

    void Start()
    {
        // Get the button on this object
        button = GetComponent<Button>();

        // Add the listener via code so you don't have to drag it in the inspector
        if (button != null)
        {
            button.onClick.AddListener(ResetDataAndReturn);
        }
    }

    public void ResetDataAndReturn()
    {
        // 1. Wipe all saved data
        PlayerPrefs.DeleteAll();

        // 2. Save the deletion (good practice to ensure it writes to disk immediately)
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs cleared! Returning to Menu...");

        // 3. Return to the MenuScene
        SceneManager.LoadScene("MenuScene");
    }
}