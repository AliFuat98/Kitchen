using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {

  /// oyuncu bir malzeme ald���nda ger�ekle�ecek event
  public event EventHandler OnPlayerGrabbedObject;

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    if (!player.HasKitchenObject()) {
      // oyuncunun eli bo�

      // yeni malzemeyi oyuncunun elinde spwan et
      KitchenObject.SpwanKitchenObject(kitchenObjectSO, player);

      // onPlayerGrabedObject animasyonun t�m client'lerde g�z�kmesi i�in
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