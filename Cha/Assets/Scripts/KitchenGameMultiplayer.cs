using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {
  public const int MAX_PLAYER_AMOUNT = 4;
  public static KitchenGameMultiplayer Instance { get; private set; }

  [SerializeField] private KitchenObjectSOList KitchenObjectSOList;
  [SerializeField] private List<Color> playerColorList = new();

  public event EventHandler OnTryingToJoinGame;

  public event EventHandler OnFailedToJoinGame;

  public event EventHandler onPlayerDataNetworkListChange;

  private NetworkList<PlayerData> playerDataNetworkList;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);

    playerDataNetworkList = new NetworkList<PlayerData>();
    playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
  }

  /// -------------------------------------------- CONNECTION ---------------------------------------

  private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
    onPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
  }

  public void StartHost() {
    NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;

    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server__OnClientDisconnectCallback;

    NetworkManager.Singleton.StartHost();
  }

  private void NetworkManager_Server__OnClientDisconnectCallback(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      PlayerData playerData = playerDataNetworkList[i];
      if (playerData.clientId == clientId) {
        // disconnected

        playerDataNetworkList.RemoveAt(i);
      }
    }
  }

  private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
    playerDataNetworkList.Add(new() {
      clientId = clientId,
      colorId = GetFirstUnusedColorId(),
    });
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

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
    NetworkManager.Singleton.StartClient();
  }

  private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
    OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
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

  /// ------------------------------------------ HELPER -------------------------------------

  public bool IsPlayerIndexConnected(int playerIndex) {
    return playerIndex < playerDataNetworkList.Count;
  }

  public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
    return playerDataNetworkList[playerIndex];
  }

  public Color GetPlayerColor(int colorId) {
    return playerColorList[colorId];
  }

  public PlayerData GetPlayerDataFromClientId(ulong clientId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientId) {
        return playerData;
      }
    }
    return default;
  }

  public int GetPlayerDataIndexFromClientId(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return i;
      }
    }
    return -1;
  }

  public PlayerData GetPlayerData() {
    return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
  }

  public void ChangePlayerColor(int colorId) {
    ChangePlayerColorServerRpc(colorId);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
    if (!IsColorAvailable(colorId)) {
      return;
    }

    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.colorId = colorId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  private bool IsColorAvailable(int colorId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.colorId == colorId) {
        // already in use
        return false;
      }
    }

    return true;
  }

  private int GetFirstUnusedColorId() {
    for (int i = 0; playerColorList.Count > 0; i++) {
      if (IsColorAvailable(i)) {
        return i;
      }
    }
    return -1;
  }

  public void KickPlayer(ulong clientId) {
    NetworkManager.Singleton.DisconnectClient(clientId);
    NetworkManager_Server__OnClientDisconnectCallback(clientId);
  }
}