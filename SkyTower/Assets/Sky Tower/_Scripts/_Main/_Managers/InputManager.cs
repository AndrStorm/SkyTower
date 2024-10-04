using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    private InputAction escapeAction;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPositionAction = playerInput.actions["TouchPosition"];
        touchPressAction = playerInput.actions["TouchPress"];
        escapeAction = playerInput.actions["Escape"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
        escapeAction.performed += EscapePressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
        escapeAction.performed -= EscapePressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        Vector2 pos = touchPositionAction.ReadValue<Vector2>();
        GameController.Instance.HandleInput(pos);
    }

    private void EscapePressed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
}
