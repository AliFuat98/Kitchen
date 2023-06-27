using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour {
  public static KitchenGameMultiplayer Instance { get; private set; }

  [SerializeField] private KitchenObjectSOList KitchenObjectSOList;

  private void Awake() {
    Instance = this;
  }

  /// ----------------------------------SPAWN------------------------------------
  public void SpwanKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
    SpwanKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
  }

  [ServerRpc(RequireOwnership = false)]
  private void SpwanKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    // listeden malzemeyinin Scriptable objesini �ek
    KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

    // malzemeyi spwan server'�n kendinde spwan et
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi t�m client'lerde spawn et
    NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
    kitchenObjectNetworkObject.Spawn(true);

    // malzemeyi �ek
    KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

    kitchenObject.SetKitchenObjectParentServerRpc(kitchenObjectParentNetworkObjectReference);
  }

  public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
    return KitchenObjectSOList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
  }

  public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
    return KitchenObjectSOList.kitchenObjectSOList[kitchenObjectSOIndex];
  }

  /// ----------------------------------DESTROY------------------------------------

  public void DestroyKitchenObject(KitchenObject kitchenObject) {
    DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
  }

  [ServerRpc(RequireOwnership = false)]
  private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
    // malzemeyi �ek
    kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
    KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    // parent'� temizle
    ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

    // malzemeyi sil
    kitchenObject.DestroyItelf();
  }

  [ClientRpc]
  private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
    // malzemeyi �ek
    kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
    KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    kitchenObject.ClearKitchenObjectOnParent();
  }
}