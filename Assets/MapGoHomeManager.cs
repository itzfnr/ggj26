using UnityEngine;
using UnityEngine.InputSystem.UI; // Required for UI Input Module
using UnityEngine.InputSystem;    // Required for Actions
using UnityEngine.SceneManagement;

public class MapGoHomeManager : MonoBehaviour
{
    private InputSystemUIInputModule uiModule;
    private InputAction cancelAction;

    void Start()
    {
        // 1. Get the component from the GameObject
        uiModule = GetComponent<InputSystemUIInputModule>();

        if (uiModule != null)
        {
            // 2. Access the Action Reference assigned to "Cancel"
            cancelAction = uiModule.cancel.action;

            // 3. Subscribe to the 'performed' event (the button press)
            cancelAction.performed += OnCancelPressed;
        }
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        // 4. Load your Menu Scene
        SceneManager.LoadScene("MenuScene");
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events when the object is destroyed to prevent memory leaks
        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancelPressed;
        }
    }
}