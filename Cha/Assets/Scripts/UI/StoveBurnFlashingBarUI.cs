using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour {
  [SerializeField] private StoveCounter stoveCounter;

  private const string IS_FLASING = "IsFlashing";

  private Animator animator;

  private void Awake() {
    animator = GetComponent<Animator>();
    animator.SetBool(IS_FLASING, false);
  }

  private void Start() {
    stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
  }

  private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
    float burnShowProgressAmount = .5f;
    bool flashing = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();
    animator.SetBool(IS_FLASING, flashing);
  }
}