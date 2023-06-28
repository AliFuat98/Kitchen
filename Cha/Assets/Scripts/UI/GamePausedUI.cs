using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePausedUI : MonoBehaviour {
  public static GamePausedUI Instance { get; private set; }

  [SerializeField] private Button ResumeButton;
  [SerializeField] private Button MainMenuButton;
  [SerializeField] private Button OptionsButton;

  private void Awake() {
    Instance = this;

    ResumeButton.onClick.AddListener(() => {
      KitchenGameManager.Instance.TogglePauseGame();
    });
    MainMenuButton.onClick.AddListener(() => {
      NetworkManager.Singleton.Shutdown();
      Loader.Load(Loader.Scene.MainMenu);
    });
    OptionsButton.onClick.AddListener(() => {
      Hide();
      OptionsUI.Instance.Show();
    });
  }

  private void Start() {
    KitchenGameManager.Instance.OnGamePausedToggled += KitchenGameManager_OnGamePausedToggled;
    Hide();
  }

  private void KitchenGameManager_OnGamePausedToggled(object sender, KitchenGameManager.OnGamePausedToggledEventArgs e) {
    if (e.isGamePaused) {
      Show();
    } else {
      Hide();
    }
  }

  public void Show() {
    gameObject.SetActive(true);
    ResumeButton.Select();
  }

  public void Hide() {
    gameObject.SetActive(false);
  }
}