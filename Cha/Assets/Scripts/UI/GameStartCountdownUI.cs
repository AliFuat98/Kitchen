using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI countdownText;

  private int previousCountdown;

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
    int countdown = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountDownToStartTimer());
    countdownText.text = countdown.ToString();

    if (countdown != previousCountdown) {
      previousCountdown = countdown;
      SoundManager.Instance.PlayCountdownSound();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}