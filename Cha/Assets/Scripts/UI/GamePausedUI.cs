using UnityEngine;
using UnityEngine.UI;

public class GamePausedUI : MonoBehaviour {
  [SerializeField] private Button ResumeButton;
  [SerializeField] private Button MainMenuButton;

  private void Awake() {
    ResumeButton.onClick.AddListener(() => {
      KitchenGameManager.Instance.TogglePauseGame();
    });
    MainMenuButton.onClick.AddListener(() => {
      Loader.Load(Loader.Scene.MainMenu);
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

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}