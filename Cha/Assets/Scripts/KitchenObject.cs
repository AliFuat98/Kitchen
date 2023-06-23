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
      Debug.LogError("counter has already an object");
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

  public void DestroyItelf() {
    // parent'ý temizle
    kitchenObjectParent.ClearKitchenObject();

    // kendini yok et
    Destroy(gameObject);
  }

  public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
    if (this is PlateKitchenObject) {
      // malzeme tabak
      plateKitchenObject = this as PlateKitchenObject;
      return true;
    }

    // malzeme tabak deðil
    plateKitchenObject = null;
    return false;
  }

  /// kendi kendini swpan et
  public static KitchenObject SpwanKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
    // malzemeyi spwan et
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi çek
    var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

    // parent'ýný ayarla
    kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

    return kitchenObject;
  }
}