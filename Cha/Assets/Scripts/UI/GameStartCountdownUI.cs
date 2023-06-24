using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI countdownText;

  private void Start() {
    KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
    Hide();
  }

  private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
    if (KitchenGameManager.Instance.IsCountDownToStartActive()) {
      Show();
    } else {
      Hide();
    }
  }

  private void Update() {
    //countdownText.text = KitchenGameManager.Instance.GetCountDownToStartTimer().ToString("#.#");
    countdownText.text = Mathf.Ceil(KitchenGameManager.Instance.GetCountDownToStartTimer()).ToString();
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}