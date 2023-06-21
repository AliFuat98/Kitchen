using UnityEngine;

public class KitchenObject : MonoBehaviour {

  /// malzemenin projede sabit duran SO'su
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// üzerinde bulunduðumuz Parent (player yada kutu)
  private IKitchenObjectParent kitchenObjectParent;

  public KitchenObjectSO GetKitchenObjectSO() {
    return kitchenObjectSO;
  }

  /// baþka Parent'a ýþýnlanma
  public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
    if (this.kitchenObjectParent != null) {
      // bir Parent'ýn üzerindeyiz

      // üzerinde bulunduðumuz eski Parent'ý temizle
      this.kitchenObjectParent.ClearKitchenObject();
    }

    // yeni Parent'ýmýzý iþaretle
    this.kitchenObjectParent = kitchenObjectParent;

    // yeni Parent'a kendimizi iþaretle
    if (kitchenObjectParent.HasKitchenObject()) {
      Debug.LogError("clear counter has already an object");
      return;
    }
    kitchenObjectParent.SetKitchenObject(this);

    // kendini yeni gelen Parent'a ýþýnla
    transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
    transform.localPosition = Vector3.zero;
  }

  /// üzerinde olduðumuz Parent'ý dön
  public IKitchenObjectParent GetKitchenObjectParent() {
    return kitchenObjectParent;
  }
}