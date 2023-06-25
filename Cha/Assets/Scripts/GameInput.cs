using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
  private const string PLAYER_PREFS_BINDINGS = "InputBindings";

  public static GameInput Instance { get; private set; }

  /// interact (E) tuþu için event
  public event EventHandler OnInteractAction;

  /// interact (F) tuþu için event
  public event EventHandler OnInteractAlternateAction;

  public enum Binding {
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
    Interact,
    InteractAlt,
    Pause,
  }

  /// new input system
  private PlayerInputActions playerInputActions;

  /// Pause (Esc) tuþu için event
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

    /// önceden yapýlan bindingleri geri çek
    if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
      playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
    }

    /// sistemi aç
    playerInputActions.Player.Enable();

    /// interact (E) tuþuna basýnca çalýþacak event
    playerInputActions.Player.Interact.performed += Interact_performed;

    /// interact (F) tuþuna basýnca çalýþacak event
    playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

    /// esc tuþu
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

  public void ResetBindings() {
    playerInputActions.RemoveAllBindingOverrides();
    PlayerPrefs.DeleteKey(PLAYER_PREFS_BINDINGS);
  }

  public string GetBindingText(Binding binding) {
    switch (binding) {
      default:
      case Binding.Move_Up:
        return playerInputActions.Player.Move.bindings[1].ToDisplayString();

      case Binding.Move_Down:
        return playerInputActions.Player.Move.bindings[2].ToDisplayString();

      case Binding.Move_Left:
        return playerInputActions.Player.Move.bindings[3].ToDisplayString();

      case Binding.Move_Right:
        return playerInputActions.Player.Move.bindings[4].ToDisplayString();

      case Binding.Interact:
        return playerInputActions.Player.Interact.bindings[0].ToDisplayString();

      case Binding.InteractAlt:
        return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();

      case Binding.Pause:
        return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
    }
  }

  public void ReBindBinding(Binding binding, Action onActionRebound) {
    playerInputActions.Player.Disable();

    InputAction inputAction = null;
    int bindingIndex;

    switch (binding) {
      case Binding.Move_Up:
        inputAction = playerInputActions.Player.Move;
        bindingIndex = 1;
        break;

      case Binding.Move_Down:
        inputAction = playerInputActions.Player.Move;
        bindingIndex = 2;
        break;

      case Binding.Move_Left:
        inputAction = playerInputActions.Player.Move;
        bindingIndex = 3;
        break;

      case Binding.Move_Right:
        inputAction = playerInputActions.Player.Move;
        bindingIndex = 4;
        break;

      case Binding.Interact:
        inputAction = playerInputActions.Player.Interact;
        bindingIndex = 0;
        break;

      case Binding.InteractAlt:
        inputAction = playerInputActions.Player.InteractAlternate;
        bindingIndex = 0;
        break;

      case Binding.Pause:
        inputAction = playerInputActions.Player.Pause;
        bindingIndex = 0;
        break;

      default:
        return;
    }

    inputAction
      .PerformInteractiveRebinding(bindingIndex)
      .OnComplete(callback => {
        callback.Dispose();
        playerInputActions.Player.Enable();
        onActionRebound();

        PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
      })
      .Start();
  }
}