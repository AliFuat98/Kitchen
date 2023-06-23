using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {

  /// sarý bar
  [SerializeField] private Image barImage;

  /// barýn üzerinde bulunduðu kesme kutusu
  [SerializeField] private CuttingCounter cuttingCounter;

  private void Start() {
    cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;
    barImage.fillAmount = 0f;
    Hide();
  }

  private void CuttingCounter_OnProgressChanged(object sender, CuttingCounter.OnProgressChangedEventArgs e) {
    barImage.fillAmount = e.progressNormalized;

    if (e.progressNormalized == 0f || e.progressNormalized == 1f) {
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