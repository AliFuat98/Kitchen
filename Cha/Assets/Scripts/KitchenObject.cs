using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {

  /// malzemenin projede sabit duran SO'su
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// �zerinde bulundu�umuz Parent (player yada kutu)
  private IKitchenObjectParent kitchenObjectParent;

  /// takip edece�imiz transform i�in script
  private FollowTransform followTransform;

  protected virtual void Awake() {
    followTransform = GetComponent<FollowTransform>();
  }

  /// ba�ka Parent'a ���nlanma
  public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
    SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
  }

  [ServerRpc(RequireOwnership = false)]
  public void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
  }

  [ClientRpc]
  private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    // parent'� �ek
    kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
    IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

    if (kitchenObjectParent.HasKitchenObject()) {
      // parent zaten var
      return;
    }

    // �zerinde bulundu�umuz eski Parent'� temizle
    this.kitchenObjectParent?.ClearKitchenObject();

    // yeni Parent'�m�z� i�aretle
    this.kitchenObjectParent = kitchenObjectParent;

    // yeni Parent'a kendimizi i�aretle
    if (kitchenObjectParent.HasKitchenObject()) {
      Debug.LogError("counter has already an object");
      return;
    }
    kitchenObjectParent.SetKitchenObject(this);

    // bir objeyi takip etmeye ba�la
    followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
  }

  /// �zerinde oldu�umuz Parent'� d�n
  public IKitchenObjectParent GetKitchenObjectParent() {
    return kitchenObjectParent;
  }

  public void DestroyItelf() {
    // kendini yok et
    Destroy(gameObject);
  }

  public void ClearKitchenObjectOnParent() {
    // parent'� temizle
    kitchenObjectParent.ClearKitchenObject();
  }

  public KitchenObjectSO GetKitchenObjectSO() {
    return kitchenObjectSO;
  }

  public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
    if (this is PlateKitchenObject) {
      // malzeme tabak
      plateKitchenObject = this as PlateKitchenObject;
      return true;
    }

    // malzeme tabak de�il
    plateKitchenObject = null;
    return false;
  }

  /// kendi kendini swpan et
  public static void SpwanKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
    KitchenGameMultiplayer.Instance.SpwanKitchenObject(kitchenObjectSO, kitchenObjectParent);
  }

  public static void DestroyKitchenObject(KitchenObject kitchenObject) {
    KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
  }
}