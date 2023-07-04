using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateSpawn;

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateRemoved;

  public class OnPlateSpawnEventArgs : EventArgs {
    public int platesSpawnedAmount;
  }

  /// bu kutunun �zerinde duracak malzeme
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// 4 saniye de bir spwan edilecek tabak
  private readonly float spawnPlatesTimerMax = 3.5f;

  private float spawnPlatesTimer;

  private readonly int platesSpawnedAmountMax = 5;
  private int platesSpawnedAmount;

  private void Update() {
    if (!IsServer) {
      return;
    }
    spawnPlatesTimer += Time.deltaTime;
    if (spawnPlatesTimer >= spawnPlatesTimerMax) {
      // yeni tabak ��kmas� i�in s�re geldi

      // s�reyi s�f�rla
      spawnPlatesTimer = 0f;

      if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
        // yeni tabak ��kabilecek kadar yer var

        // tabak g�rseli g�ster
        SpawnPlateClientRpc();
      }
    }
  }

  [ClientRpc]
  private void SpawnPlateClientRpc() {
    OnPlateSpawn?.Invoke(this, new OnPlateSpawnEventArgs {
      platesSpawnedAmount = platesSpawnedAmount,
    });
    platesSpawnedAmount++;
  }

  public override void Interact(Player player) {
    if (!player.HasKitchenObject()) {
      // oyuncunun eli bo�

      if (platesSpawnedAmount > 0) {
        // kutunun �zerinde tabak var

        // bir adet tabak g�rselini sil
        InteractLogicServerRpc(player.GetNetworkObject());

        //  oyuncuya tabak ver
        KitchenObject.SpwanKitchenObject(kitchenObjectSO, player);
      }
    } else {
      // oyuncunun eli dolu

      //---
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void InteractLogicServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    InteractLogicClientRpc(kitchenObjectParentNetworkObjectReference);
  }

  [ClientRpc]
  private void InteractLogicClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    // parent'� �ek
    kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
    IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

    if (kitchenObjectParent.HasKitchenObject()) {
      // elinde tabak zaten var ��k
      return;
    }

    // bir adet tabak g�rselini sil
    platesSpawnedAmount--;
    if (platesSpawnedAmount < 0) {
      platesSpawnedAmount = 0;
      return;
    }
    OnPlateRemoved?.Invoke(this, new OnPlateSpawnEventArgs {
      platesSpawnedAmount = platesSpawnedAmount
    });
  }
}