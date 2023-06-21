using System;
using UnityEngine;

public class GameInput : MonoBehaviour {

  /// interact (E) tu�u i�in event
  public event EventHandler OnInteractAction;

  /// interact (F) tu�u i�in event
  public event EventHandler OnInteractAlternateAction;

  /// new input system
  private PlayerInputActions playerInputActions;

  private void Awake() {
    /// open new input system
    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();

    /// interact (E) tu�una bas�nca �al��acak event
    playerInputActions.Player.Interact.performed += Interact_performed;

    /// interact (F) tu�una bas�nca �al��acak event
    playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed; ;
  }

  private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
  }

  private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnInteractAction?.Invoke(this, EventArgs.Empty);
  }

  public Vector2 GetMovementVectorNormalized() {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

    inputVector = inputVector.normalized;

    return inputVector;
  }
}