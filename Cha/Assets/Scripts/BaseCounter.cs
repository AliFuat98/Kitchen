using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun �zerindeki malzeme
  private KitchenObject kitchenObject;

  public virtual void Interact(Player player) {
    Debug.LogError("BaseCounter.interact");
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