using UnityEngine;

public class KitchenObject : MonoBehaviour {
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// üzerinde bulunduðumuz kutu
  private ClearCounter clearCounter;

  public KitchenObjectSO GetKitchenObjectSO() {
    return kitchenObjectSO;
  }

  /// baþka kutuya ýþýnlanma
  public void SetClearCounter(ClearCounter clearCounter) {
    if (this.clearCounter != null) {
      // bir kutunun üzerindeyiz

      // üzerinde bulunduðumuz eski kutuyu temizle
      this.clearCounter.ClearKitchenObject();
    }

    // yeni kutumuzu iþaretle
    this.clearCounter = clearCounter;

    // yeni kutuya kendimizi iþaretle
    if (clearCounter.HasKitchenObject()) {
      Debug.LogError("clear counter has already an object");
      return;
    }
    clearCounter.SetKitchenObject(this);

    // kendini yeni gelen kutuya ýþýnla
    transform.parent = clearCounter.GetKitchenObjectFollowTransform();
    transform.localPosition = Vector3.zero;
  }

  /// üzerinde olduðumuz kutuyu dön
  public ClearCounter GetClearCounter() {
    return clearCounter;
  }
}