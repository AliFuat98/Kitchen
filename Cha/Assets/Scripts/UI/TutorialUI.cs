using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI keyInteractText;
  [SerializeField] private TextMeshProUGUI keyInteractAltText;
  [SerializeField] private TextMeshProUGUI keyPauseText;

  [SerializeField] private TextMeshProUGUI keyInteractGamepadText;
  [SerializeField] private TextMeshProUGUI keyInteractAltGamepadText;
  [SerializeField] private TextMeshProUGUI keyPauseGamepadText;

  private void Start() {
    GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
    KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
    UpdateVisual();
    Show();
  }

  private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
    if (KitchenGameManager.Instance.IsLocalPlayerReady()) {
      Hide();
    }
  }

  private void GameInput_OnBindingRebind(object sender, System.EventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
    keyInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
    keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

    keyInteractGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
    keyInteractAltGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlt);
    keyPauseGamepadText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}