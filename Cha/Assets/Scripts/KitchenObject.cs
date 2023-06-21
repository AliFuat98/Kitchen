using UnityEngine;

public class KitchenObject : MonoBehaviour {

  /// malzemenin projede sabit duran SO'su
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// �zerinde bulundu�umuz Parent (player yada kutu)
  private IKitchenObjectParent kitchenObjectParent;

  public KitchenObjectSO GetKitchenObjectSO() {
    return kitchenObjectSO;
  }

  /// ba�ka Parent'a ���nlanma
  public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
    if (this.kitchenObjectParent != null) {
      // bir Parent'�n �zerindeyiz

      // �zerinde bulundu�umuz eski Parent'� temizle
      this.kitchenObjectParent.ClearKitchenObject();
    }

    // yeni Parent'�m�z� i�aretle
    this.kitchenObjectParent = kitchenObjectParent;

    // yeni Parent'a kendimizi i�aretle
    if (kitchenObjectParent.HasKitchenObject()) {
      Debug.LogError("clear counter has already an object");
      return;
    }
    kitchenObjectParent.SetKitchenObject(this);

    // kendini yeni gelen Parent'a ���nla
    transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
    transform.localPosition = Vector3.zero;
  }

  /// �zerinde oldu�umuz Parent'� d�n
  public IKitchenObjectParent GetKitchenObjectParent() {
    return kitchenObjectParent;
  }
}