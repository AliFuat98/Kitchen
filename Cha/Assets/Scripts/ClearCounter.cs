using UnityEngine;

public class ClearCounter : MonoBehaviour {

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun üzerindeki malzeme
  private KitchenObject kitchenObject;

  /// taþýma yapýcaðýmýz ikinci kutu (TEST)
  [SerializeField] private ClearCounter secondClearCounter;

  [SerializeField] private bool testing;

  private void Update() {
    if (testing && Input.GetKeyDown(KeyCode.T)) {
      if (kitchenObject != null) {
        // kutu üzeri dolu

        // ikici test kutusuna ýþýnla
        kitchenObject.SetClearCounter(secondClearCounter);
      }
    }
  }

  public void Interact() {
    if (kitchenObject == null) {
      // kutunun üzeri boþ

      // malzemeyi oluþtur
      Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
      kitchenObjectTransform.localPosition = Vector3.zero;

      // malzemenin hangi kutunun üzerinde olduðunu iþaretle
      kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this);
    } else {
      // kutunun üzeri dolu

      // hangi kutu bastýr
      Debug.Log(kitchenObject.GetClearCounter());
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