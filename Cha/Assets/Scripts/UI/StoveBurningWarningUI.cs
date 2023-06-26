using UnityEngine;

public class StoveBurningWarningUI : MonoBehaviour {
  [SerializeField] private StoveCounter stoveCounter;

  private void Start() {
    stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

    Hide();
  }

  private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
    float burnShowProgressAmount = .5f;
    bool show = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();
    if (show) {
      Show();
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