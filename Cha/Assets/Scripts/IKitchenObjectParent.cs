using UnityEngine;

public interface IKitchenObjectParent {

  /// parent'ın spawn noktasını dön
  public Transform GetKitchenObjectFollowTransform();

  /// parent'ın üzerindeki malzemeyi değiştir
  public void SetKitchenObject(KitchenObject kitchenObject);

  /// parent'ın üzerindeki malzemeyi dön
  public KitchenObject GetKitchenObject();

  /// parent'ın üzerini temizle
  public void ClearKitchenObject();

  /// parent'ın üzerinde bir malzeme var mı?
  public bool HasKitchenObject();
}