using UnityEngine;

public class KitchenObject : MonoBehaviour {
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// �zerinde bulundu�umuz kutu
  private ClearCounter clearCounter;

  public KitchenObjectSO GetKitchenObjectSO() {
    return kitchenObjectSO;
  }

  /// ba�ka kutuya ���nlanma
  public void SetClearCounter(ClearCounter clearCounter) {
    if (this.clearCounter != null) {
      // bir kutunun �zerindeyiz

      // �zerinde bulundu�umuz eski kutuyu temizle
      this.clearCounter.ClearKitchenObject();
    }

    // yeni kutumuzu i�aretle
    this.clearCounter = clearCounter;

    // yeni kutuya kendimizi i�aretle
    if (clearCounter.HasKitchenObject()) {
      Debug.LogError("clear counter has already an object");
      return;
    }
    clearCounter.SetKitchenObject(this);

    // kendini yeni gelen kutuya ���nla
    transform.parent = clearCounter.GetKitchenObjectFollowTransform();
    transform.localPosition = Vector3.zero;
  }

  /// �zerinde oldu�umuz kutuyu d�n
  public ClearCounter GetClearCounter() {
    return clearCounter;
  }
}