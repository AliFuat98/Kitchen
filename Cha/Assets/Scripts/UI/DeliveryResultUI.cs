using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {
  [SerializeField] private Image backgroundImage;
  [SerializeField] private Image iconImage;
  [SerializeField] private TextMeshProUGUI messageText;

  [SerializeField] private Color successColor;
  [SerializeField] private Color failColor;

  [SerializeField] private Sprite successSprite;
  [SerializeField] private Sprite failSprite;

  private Animator animator;
  private const string DELIVERY_POPUP = "DeliveryPopup";

  private void Awake() {
    animator = GetComponent<Animator>();
  }

  private void Start() {
    DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
    DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

    gameObject.SetActive(false);
  }

  private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
    backgroundImage.color = failColor;
    iconImage.sprite = failSprite;
    messageText.text = "DELIVERY\nFAILED";
    animator.SetTrigger(DELIVERY_POPUP);
  }

  private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
    gameObject.SetActive(true);
    backgroundImage.color = successColor;
    iconImage.sprite = successSprite;
    messageText.text = "DELIVERY\nSUCCESS";
    animator.SetTrigger(DELIVERY_POPUP);
  }
}