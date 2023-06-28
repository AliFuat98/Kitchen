using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {
  public const int MAX_PLAYER_AMOUNT = 4;
  public static KitchenGameMultiplayer Instance { get; private set; }

  [SerializeField] private KitchenObjectSOList KitchenObjectSOList;

  public event EventHandler OnTryingToJoinGame;

  public event EventHandler OnFailedToJoinGame;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);
  }

  public void StartHost() {
    NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;

    NetworkManager.Singleton.StartHost();
  }

  public override void OnNetworkDespawn() {
    NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
  }

  private void NetworkManager_ConnectionApprovalCallback(
      NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
      NetworkManager.ConnectionApprovalResponse connectionApprovalResponse
    ) {
    if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is already Starterd ";
      return;
    }

    if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is full ";
      return;
    }

    connectionApprovalResponse.Approved = true;
  }

  public void StartClient() {
    OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    NetworkManager.Singleton.StartClient();
  }

  private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
    OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
  }

  /// ----------------------------------SPAWN------------------------------------
  public void SpwanKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
    SpwanKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
  }

  [ServerRpc(RequireOwnership = false)]
  private void SpwanKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    // listeden malzemeyinin Scriptable objesini çek
    KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

    // malzemeyi spwan server'ýn kendinde spwan et
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi tüm client'lerde spawn et
    NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
    kitchenObjectNetworkObject.Spawn(true);

    // malzemeyi çek
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
    // malzemeyi çek
    kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
    KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    // parent'ý temizle
    ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

    // malzemeyi sil
    kitchenObject.DestroyItelf();
  }

  [ClientRpc]
  private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
    // malzemeyi çek
    kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
    KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    kitchenObject.ClearKitchenObjectOnParent();
  }
}