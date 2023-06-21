using UnityEngine;

public class ClearCounter : MonoBehaviour {

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// malzemeyi nerde spawn edicez = kutunun tepesi
  [SerializeField] private Transform counterTopPoint;

  /// kutunun �zerindeki malzeme
  private KitchenObject kitchenObject;

  /// ta��ma yap�ca��m�z ikinci kutu (TEST)
  [SerializeField] private ClearCounter secondClearCounter;

  [SerializeField] private bool testing;

  private void Update() {
    if (testing && Input.GetKeyDown(KeyCode.T)) {
      if (kitchenObject != null) {
        // kutu �zeri dolu

        // ikici test kutusuna ���nla
        kitchenObject.SetClearCounter(secondClearCounter);
      }
    }
  }

  public void Interact() {
    if (kitchenObject == null) {
      // kutunun �zeri bo�

      // malzemeyi olu�tur
      Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
      kitchenObjectTransform.localPosition = Vector3.zero;

      // malzemenin hangi kutunun �zerinde oldu�unu i�aretle
      kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this);
    } else {
      // kutunun �zeri dolu

      // hangi kutu bast�r
      Debug.Log(kitchenObject.GetClearCounter());
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