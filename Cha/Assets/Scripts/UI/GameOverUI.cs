using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI recipesDeliveredText;
  [SerializeField] private Button playAgainButton;

  private void Start() {
    KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
    Hide();

    playAgainButton.onClick.AddListener(() => {
      NetworkManager.Singleton.Shutdown();
      Loader.Load(Loader.Scene.MainMenuScene);
    });
  }

  private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
    if (KitchenGameManager.Instance.IsGameOver()) {
      recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullDeliverAmount().ToString();
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