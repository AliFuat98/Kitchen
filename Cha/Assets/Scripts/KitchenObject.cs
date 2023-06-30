using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {

  /// malzemenin projede sabit duran SO'su
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// üzerinde bulunduðumuz Parent (player yada kutu)
  private IKitchenObjectParent kitchenObjectParent;

  /// takip edeceðimiz transform için script
  private FollowTransform followTransform;

  protected virtual void Awake() {
    followTransform = GetComponent<FollowTransform>();
  }

  /// baþka Parent'a ýþýnlanma
  public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
    SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
  }

  [ServerRpc(RequireOwnership = false)]
  public void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
  }

  [ClientRpc]
  private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    // parent'ý çek
    kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
    IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

    if (kitchenObjectParent.HasKitchenObject()) {
      // parent zaten var
      return;
    }

    // üzerinde bulunduðumuz eski Parent'ý temizle
    this.kitchenObjectParent?.ClearKitchenObject();

    // yeni Parent'ýmýzý iþaretle
    this.kitchenObjectParent = kitchenObjectParent;

    // yeni Parent'a kendimizi iþaretle
    if (kitchenObjectParent.HasKitchenObject()) {
      Debug.LogError("counter has already an object");
      return;
    }
    kitchenObjectParent.SetKitchenObject(this);

    // bir objeyi takip etmeye baþla
    followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
  }

  /// üzerinde olduðumuz Parent'ý dön
  public IKitchenObjectParent GetKitchenObjectParent() {
    return kitchenObjectParent;
  }

  public void DestroyItelf() {
    // kendini yok et
    Destroy(gameObject);
  }

  public void ClearKitchenObjectOnParent() {
    // parent'ý temizle
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

    // malzeme tabak deðil
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