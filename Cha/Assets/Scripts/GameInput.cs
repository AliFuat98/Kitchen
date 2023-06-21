using System;
using UnityEngine;

public class GameInput : MonoBehaviour {

  /// interact (E) tu�u i�in event
  public event EventHandler OnInteratAction;

  /// new input system
  private PlayerInputActions playerInputActions;

  private void Awake() {
    /// open new input system
    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();

    /// interact (E) tu�una bas�nca �al��acak event
    playerInputActions.Player.Interact.performed += Interact_performed;
  }

  private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnInteratAction?.Invoke(this, EventArgs.Empty);
  }

  public Vector2 GetMovementVectorNormalized() {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

    inputVector = inputVector.normalized;

    return inputVector;
  }
}