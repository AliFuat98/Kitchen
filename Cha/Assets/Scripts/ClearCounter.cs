using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent {

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun �zerindeki malzeme
  private KitchenObject kitchenObject;

  public void Interact(Player player) {
    if (kitchenObject == null) {
      // kutunun �zeri bo�

      // malzemeyi olu�tur
      Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
      kitchenObjectTransform.localPosition = Vector3.zero;

      // malzemenin hangi kutunun �zerinde oldu�unu i�aretle
      kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
    } else {
      // kutunun �zeri dolu

      // malzemeyi oyuncuya ver
      kitchenObject.SetKitchenObjectParent(player);
    }
  }

  /// kutunun spawn noktas�n� d�n
  public Transform GetKitchenObjectFollowTransform() {
    return counterTopPoint;
  }

  /// kutunun �zerindeki malzemeyi de�i�tir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
  }

  /// kutunun �zerindeki malzemeyi d�n
  public KitchenObject GetKitchenObject() {
    return kitchenObject;
  }

  /// kutunun �zerini temizle
  public void ClearKitchenObject() {
    kitchenObject = null;
  }

  /// kutunun �zerinde bir malzeme var m�?
  public bool HasKitchenObject() {
    return kitchenObject != null;
  }
}