using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
  public static OptionsUI Instance { get; private set; }

  [SerializeField] private Button musicButton;
  [SerializeField] private Button soundEffectsButton;
  [SerializeField] private Button backButton;

  [SerializeField] private Button MoveUpButton;
  [SerializeField] private Button MoveDownButton;
  [SerializeField] private Button MoveLeftButton;
  [SerializeField] private Button MoveRightButton;
  [SerializeField] private Button InteractButton;
  [SerializeField] private Button InteractAltButton;
  [SerializeField] private Button PauseButton;

  [SerializeField] private Button ResetBindinsButton;

  [SerializeField] private TextMeshProUGUI musicText;
  [SerializeField] private TextMeshProUGUI soundEffectsText;

  [SerializeField] private TextMeshProUGUI MoveUpText;
  [SerializeField] private TextMeshProUGUI MoveDownText;
  [SerializeField] private TextMeshProUGUI MoveLeftText;
  [SerializeField] private TextMeshProUGUI MoveRightText;
  [SerializeField] private TextMeshProUGUI InteractText;
  [SerializeField] private TextMeshProUGUI InteractAltText;
  [SerializeField] private TextMeshProUGUI PauseText;

  [SerializeField] private Transform PressToRebindingKeyTransform;

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

    MoveUpButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Up);
    });
    MoveDownButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Down);
    });
    MoveLeftButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Left);
    });
    MoveRightButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Move_Right);
    });

    InteractButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Interact);
    });
    InteractAltButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.InteractAlt);
    });
    PauseButton.onClick.AddListener(() => {
      RebindBinding(GameInput.Binding.Pause);
    });

    ResetBindinsButton.onClick.AddListener(() => {
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

    MoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
    MoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
    MoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
    MoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);

    InteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
    InteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlt);
    PauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
  }

  public void Show() {
    gameObject.SetActive(true);
  }

  public void Hide() {
    gameObject.SetActive(false);
  }

  public void ShowRebindkeyTransform() {
    PressToRebindingKeyTransform.gameObject.SetActive(true);
  }

  public void HideRebindkeyTransform() {
    PressToRebindingKeyTransform.gameObject.SetActive(false);
  }

  private void RebindBinding(GameInput.Binding binding) {
    ShowRebindkeyTransform();
    GameInput.Instance.ReBindBinding(binding, () => {
      HideRebindkeyTransform();
      UpdateVisual();
    });
  }
}