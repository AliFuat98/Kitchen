using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {

  /// sarý bar
  [SerializeField] private Image barImage;

  /// barýn üzerinde bulunduðu kesme kutusu
  [SerializeField] private GameObject hasProgressGameObject;

  private IHasProgress hasProgress;

  private void Start() {
    hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
    if (hasProgress == null) {
      Debug.LogError($"{hasProgressGameObject.name} this game object dont have IhasProgress interface");
    }

    hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
    barImage.fillAmount = 0f;
    Hide();
  }

  private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
    barImage.fillAmount = e.progressNormalized;
    if (e.progressNormalized == 0f || e.progressNormalized >= 1f) {
      // full ya da empty sakla
      Hide();
    } else {
      Show();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}