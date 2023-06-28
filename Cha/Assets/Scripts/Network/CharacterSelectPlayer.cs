using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {
  [SerializeField] private int playerIndex;
  [SerializeField] private Button kickButton;
  [SerializeField] private GameObject readyGameObject;
  [SerializeField] private PlayerVisual playerVisual;

  private void Awake() {
    kickButton.onClick.AddListener(() => {
      PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
    });
  }

  private void Start() {
    KitchenGameMultiplayer.Instance.onPlayerDataNetworkListChange += KitchenGameMultiplayer_onPlayerDataNetworkListChange;
    CharacterSelectReady.Instance.onReadyChanged += CharacterSelectReady_onReadyChanged;

    kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

    if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      if (NetworkManager.ServerClientId == playerData.clientId) {
        // kendini atma yok
        kickButton.gameObject.SetActive(false);
      }
    }

    UpdatePlayer();
  }

  private void OnDestroy() {
    KitchenGameMultiplayer.Instance.onPlayerDataNetworkListChange -= KitchenGameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void CharacterSelectReady_onReadyChanged(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void KitchenGameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void UpdatePlayer() {
    if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      Show();

      PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

      playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
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