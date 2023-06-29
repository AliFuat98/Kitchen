using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
  [SerializeField] private Button closeButton;
  [SerializeField] private TextMeshProUGUI messageText;

  private void Awake() {
    closeButton.onClick.AddListener(() => {
      Hide();
    });
  }

  private void Start() {
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
    KitchenGameLobby.Instance.OnCreateLobbyFailedStarted += KitchenGameLobby_OnCreateLobbyFailedStarted;
    KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;

    KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
    KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
    KitchenGameLobby.Instance.OnQuickJoinFailedStarted += KitchenGameLobby_OnQuickJoinFailedStarted;
    Hide();
  }

  private void KitchenGameLobby_OnQuickJoinFailedStarted(object sender, System.EventArgs e) {
    ShowMessage("Could Not found a lobby to Quick join!");
  }

  private void KitchenGameLobby_OnJoinFailed(object sender, System.EventArgs e) {
    ShowMessage("Creating lobby...");
  }

  private void KitchenGameLobby_OnJoinStarted(object sender, System.EventArgs e) {
    ShowMessage("Joining lobby..");
  }

  private void KitchenGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
    ShowMessage("Creating lobby...");
  }

  private void KitchenGameLobby_OnCreateLobbyFailedStarted(object sender, System.EventArgs e) {
    ShowMessage("Fail to Create lobby!");
  }

  private void OnDestroy() {
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
  }

  private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
    if (NetworkManager.Singleton.DisconnectReason == "") {
      ShowMessage("Failed to Connect");
    } else {
      ShowMessage(NetworkManager.Singleton.DisconnectReason);
    }
  }

  private void ShowMessage(string message) {
    Show();

    messageText.text = message;
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}