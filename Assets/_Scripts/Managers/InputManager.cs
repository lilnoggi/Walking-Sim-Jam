using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Singeton Instance
    public static InputManager Instance { get; private set; }

    // Reference to the Input Actions Class
    private InputSystem_Actions _inputActions;

    //Public getter to protect the variable from being overwritten externally
    public InputSystem_Actions Actions => _inputActions;

    // ------------------------------------------------------------

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _inputActions = new InputSystem_Actions();

        _inputActions.Global.SwitchMode.performed += ToggleControlMode;
    }

    private void OnEnable()
    {
        // Global must ALWAYS remain active for the camera and spacebar
        _inputActions.Global.Enable();

        // Default the game to Leg mode on startup
        _inputActions.Legs.Enable();
        _inputActions.Arms.Disable();
    }

    private void OnDisable()
    {
        // Clean up memory when the object is destroyed
        _inputActions.Global.Disable();
        _inputActions.Legs.Disable();
        _inputActions.Arms.Disable();
    }

    private void ToggleControlMode(InputAction.CallbackContext context)
    {
        // If legs are currently active, swap to arms
        if (_inputActions.Legs.enabled)
        {
            _inputActions.Legs.Disable();
            _inputActions.Arms.Enable();
            Debug.Log("Switched to ARM Mode");
        }
        else
        {
            // Otherwise, swap back to legs
            _inputActions.Arms.Disable();
            _inputActions.Legs.Enable();
            Debug.Log("Switched to LEG Mode");
        }
    }
}
