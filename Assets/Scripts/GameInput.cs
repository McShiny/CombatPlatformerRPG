using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpPreformed;
    public event EventHandler OnDashPreformed;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Dash.performed += Dash_performed;
        playerInputActions.Player.Jump.performed += Jump_performed;
    }

    private void OnDestroy() {
        playerInputActions.Player.Dash.performed -= Dash_performed;

        playerInputActions.Dispose();
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnDashPreformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpPreformed?.Invoke(this, EventArgs.Empty);
    }

    public float GetMovementVectorNormalized() {
        float inputVector = playerInputActions.Player.Move.ReadValue<float>();

        return inputVector;
    }

    public bool GetJumpDown() {
        return playerInputActions.Player.Jump.ReadValue<float>() == 1;
    }

    public bool IsWalking() {
        return playerInputActions.Player.Walk.ReadValue<float>() == 1;
    }
}
