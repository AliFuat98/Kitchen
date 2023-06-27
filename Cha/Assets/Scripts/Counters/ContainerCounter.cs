using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {

  /// oyuncu bir malzeme aldýðýnda gerçekleþecek event
  public event EventHandler OnPlayerGrabbedObject;

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    if (!player.HasKitchenObject()) {
      // oyuncunun eli boþ

      // yeni malzemeyi oyuncunun elinde spwan et
      KitchenObject.SpwanKitchenObject(kitchenObjectSO, player);

      // onPlayerGrabedObject animasyonun tüm client'lerde gözükmesi için
      InteractLogicServerRpc();
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void InteractLogicServerRpc() {
    InteractLogicClientRpc();
  }

  [ClientRpc]
  private void InteractLogicClientRpc() {
    OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
  }
}