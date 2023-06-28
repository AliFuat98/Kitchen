using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectedSingleUI : MonoBehaviour {
  [SerializeField] private int colorId;
  [SerializeField] private Image image;
  [SerializeField] private GameObject selectedGameObject;

  private void Awake() {
    GetComponent<Button>().onClick.AddListener(() => {
      KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
    });
  }

  private void Start() {
    KitchenGameMultiplayer.Instance.onPlayerDataNetworkListChange += KitchenGameMultiplayer_onPlayerDataNetworkListChange;
    image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
    UpdateIsSelected();
  }

  private void OnDestroy() {
    KitchenGameMultiplayer.Instance.onPlayerDataNetworkListChange -= KitchenGameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void KitchenGameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdateIsSelected();
  }

  private void UpdateIsSelected() {
    if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
      selectedGameObject.SetActive(true);
    } else {
      selectedGameObject.SetActive(false);
    }
  }
}