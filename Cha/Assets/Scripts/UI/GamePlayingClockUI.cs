using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour {
  [SerializeField] private Image timerImage;

  KitchenGameManager gameManager;

  private void Start() {
    gameManager = KitchenGameManager.Instance;
  }

  private void Update() {
    timerImage.fillAmount = gameManager.GetGamePlayingTimerNormalized();
  }
}