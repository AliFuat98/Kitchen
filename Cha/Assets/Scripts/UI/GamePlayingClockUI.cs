using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {
  [SerializeField] private Image timerImage;
  [SerializeField] private TextMeshProUGUI deliveryCountText;

  KitchenGameManager gameManager;

  private void Start() {
    gameManager = KitchenGameManager.Instance;

    DeliveryManager.Instance.OnDeliveryAmountChanged += DeliveryManager_OnDeliveryAmountChanged;
  }

  private void DeliveryManager_OnDeliveryAmountChanged(object sender, System.EventArgs e) {
    deliveryCountText.text = DeliveryManager.Instance.GetSuccessfullDeliverAmount().ToString();
  }

  private void Update() {
    timerImage.fillAmount = gameManager.GetGamePlayingTimerNormalized();
  }
}