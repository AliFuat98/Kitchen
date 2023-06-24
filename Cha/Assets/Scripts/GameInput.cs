using System;
using UnityEngine;

public class GameInput : MonoBehaviour {
  public static GameInput Instance { get; private set; }

  /// interact (E) tu�u i�in event
  public event EventHandler OnInteractAction;

  /// interact (F) tu�u i�in event
  public event EventHandler OnInteractAlternateAction;

  /// new input system
  private PlayerInputActions playerInputActions;

  /// Pause (Esc) tu�u i�in event
  public event EventHandler OnPauseAction;

  private void OnDestroy() {
    playerInputActions.Player.Interact.performed += Interact_performed;
    playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
    playerInputActions.Player.Pause.performed += Pause_performed;

    playerInputActions.Dispose();
  }

  private void Awake() {
    Instance = this;

    /// open new input system
    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();

    /// interact (E) tu�una bas�nca �al��acak event
    playerInputActions.Player.Interact.performed += Interact_performed;

    /// interact (F) tu�una bas�nca �al��acak event
    playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

    /// esc tu�u
    playerInputActions.Player.Pause.performed += Pause_performed;
  }

  private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnPauseAction?.Invoke(this, EventArgs.Empty);
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