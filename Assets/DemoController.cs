using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoController : MonoBehaviour
{
    // Static reference to hold the one and only instance
    private static DemoController instance;

    private PlayerInput playerInput;
    private InputAction resetOneAction;
    private InputAction resetTwoAction;

    private bool isResetOnePressed = false;
    private bool isResetTwoPressed = false;

    void Awake()
    {
        // --- SINGLETON & PERSISTENCE LOGIC ---
        if (instance != null && instance != this)
        {
            // If an instance already exists, destroy this new one and stop
            Destroy(gameObject);
            return;
        }

        // Set this as the instance and protect it from scene loads
        instance = this;
        DontDestroyOnLoad(gameObject);

        // --- INPUT SETUP ---
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            resetOneAction = playerInput.actions["DebugDemoResetOne"];
            resetTwoAction = playerInput.actions["DebugDemoResetTwo"];
        }
    }

    void OnEnable()
    {
        // Only subscribe if we are the valid instance
        if (instance != this) return;

        resetOneAction.performed += ctx => isResetOnePressed = true;
        resetOneAction.canceled += ctx => isResetOnePressed = false;

        resetTwoAction.performed += ctx => isResetTwoPressed = true;
        resetTwoAction.canceled += ctx => isResetTwoPressed = false;
    }

    void Update()
    {
        // Combo check
        if (isResetOnePressed && isResetTwoPressed)
        {
            // Reset flags so it doesn't trigger multiple times in one frame
            isResetOnePressed = false;
            isResetTwoPressed = false;

            SceneManager.LoadScene("DebugScene");
        }
    }

    void OnDisable()
    {
        // Cleanup subscriptions
        if (instance == this && resetOneAction != null)
        {
            resetOneAction.performed -= ctx => isResetOnePressed = true;
            resetOneAction.canceled -= ctx => isResetOnePressed = false;
            resetTwoAction.performed -= ctx => isResetTwoPressed = true;
            resetTwoAction.canceled -= ctx => isResetTwoPressed = false;
        }
    }
}