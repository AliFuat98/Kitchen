using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
  public static OptionsUI Instance { get; private set; }

  [SerializeField] private Button musicButton;
  [SerializeField] private Button soundEffectsButton;
  [SerializeField] private Button backButton;

  [SerializeField] private Button moveUpButton;
  [SerializeField] private Button moveDownButton;
  [SerializeField] private Button moveLeftButton;
  [SerializeField] private Button moveRightButton;
  [SerializeField] private Button interactButton;
  [SerializeField] private Button interactAltButton;
  [SerializeField] private Button pauseButton;
  [SerializeField] private Button gamepadInteractButton;
  [SerializeField] private Button gamepadInteractAltButton;
  [SerializeField] private Button gamepadPauseButton;

  [SerializeField] private Button resetBindinsButton;

  [SerializeField] private TextMeshProUGUI musicText;
  [SerializeField] private TextMeshProUGUI soundEffectsText;

  [SerializeField] private TextMeshProUGUI moveUpText;
  [SerializeField] private TextMeshProUGUI moveDownText;
  [SerializeField] private TextMeshProUGUI moveLeftText;
  [SerializeField] private TextMeshProUGUI moveRightText;
  [SerializeField] private TextMeshProUGUI interactText;
  [SerializeField] private TextMeshProUGUI interactAltText;
  [SerializeField] private TextMeshProUGUI pauseText;
  [SerializeField] private TextMeshProUGUI gamepadInteractText;
  [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
  [SerializeField] private TextMeshProUGUI gamepadPauseText;

  [SerializeField] private Transform pressToRebindingKeyTransform;

  private void Awake() {
    Instance = this;

    musicButton.onClick.AddListener(() => {
      SoundManager.Instance.ChangeVolume();
      UpdateVisual();
    });
    soundEffectsButton.onClick.AddListener(() => {
      MusicManager.Instance.ChangeVolume();
      UpdateVisual();
    });

    backButton.onClick.AddListener(() => {
      Hide();
      GamePausedUI.Instance.Show();
    });

    moveUpButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Up);
    });
    moveDownButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Down);
    });
    moveLeftButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Left);
    });
    moveRightButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Right);
    });

    interactButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Interact);
    });
    interactAltButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.InteractAlt);
    });
    pauseButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Pause);
    });

    gamepadInteractButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Gamepad_Interact);
    });
    gamepadInteractAltButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Gamepad_InteractAlt);
    });
    gamepadPauseButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Gamepad_Pause);
    });

    resetBindinsButton.onClick.AddListener(() => {
      GameInput.Instance.ResetBindings();
      UpdateVisual();
    });
  }

  private void Start() {
    KitchenGameManager.Instance.OnGamePausedToggled += KitchenGameManager_OnGamePausedToggled;
    HideRebindkeyTransform();
    Hide();
    UpdateVisual();
  }

  private void KitchenGameManager_OnGamePausedToggled(object sender, KitchenGameManager.OnGamePausedToggledEventArgs e) {
    if (e.isGamePaused) {
    } else {
      Hide();
    }
  }

  private void UpdateVisual() {
    soundEffectsText.text = $"Sound Effects: {Mathf.Round(SoundManager.Instance.getVolume() * 10f)}";
    musicText.text = $"Music: {Mathf.Round(MusicManager.Instance.getVolume() * 10f)}";

    moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
    moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
    moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
    moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);

    interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
    interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
    pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

    gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
    gamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlt);
    gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
  }

  public void Show() {
    gameObject.SetActive(true);
    soundEffectsButton.Select();
  }

  public void Hide() {
    gameObject.SetActive(false);
  }

  public void ShowRebindkeyTransform() {
    pressToRebindingKeyTransform.gameObject.SetActive(true);
  }

  public void HideRebindkeyTransform() {
    pressToRebindingKeyTransform.gameObject.SetActive(false);
  }

  private void RebindBinding(GameInput.Binding binding) {
    ShowRebindkeyTransform();
    GameInput.Instance.ReBindBinding(binding, () => {
      HideRebindkeyTransform();
      UpdateVisual();
    });
  }
}