using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent {

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun üzerindeki malzeme
  private KitchenObject kitchenObject;

  public void Interact(Player player) {
    if (kitchenObject == null) {
      // kutunun üzeri boþ

      // malzemeyi oluþtur
      Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
      kitchenObjectTransform.localPosition = Vector3.zero;

      // malzemenin hangi kutunun üzerinde olduðunu iþaretle
      kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
    } else {
      // kutunun üzeri dolu

      // malzemeyi oyuncuya ver
      kitchenObject.SetKitchenObjectParent(player);
    }
  }

  /// kutunun spawn noktasýný dön
  public Transform GetKitchenObjectFollowTransform() {
    return counterTopPoint;
  }

  /// kutunun üzerindeki malzemeyi deðiþtir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
  }

  /// kutunun üzerindeki malzemeyi dön
  public KitchenObject GetKitchenObject() {
    return kitchenObject;
  }

  /// kutunun üzerini temizle
  public void ClearKitchenObject() {
    kitchenObject = null;
  }

  /// kutunun üzerinde bir malzeme var mý?
  public bool HasKitchenObject() {
    return kitchenObject != null;
  }
}