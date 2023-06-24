using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

  /// ses için event
  public static event EventHandler OnAnyObjectPlacedHere;

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun üzerindeki malzeme
  private KitchenObject kitchenObject;

  public virtual void Interact(Player player) {
    Debug.Log("BaseCounter.interact");
  }

  public virtual void InteractAlternate(Player player) {
    Debug.Log("BaseCounter.interact");
  }

  /// kutunun spawn noktasýný dön
  public Transform GetKitchenObjectFollowTransform() {
    return counterTopPoint;
  }

  /// kutunun üzerindeki malzemeyi deðiþtir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
    if (kitchenObject != null) {
      OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
    }
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