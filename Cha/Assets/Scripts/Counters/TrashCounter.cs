using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter {

  /// ses i�in event
  public static event EventHandler OnAnyObjectTrashed;

  public static new void ResetStaticData() {
    OnAnyObjectTrashed = null;
  }

  public override void Interact(Player player) {
    if (player.HasKitchenObject()) {
      // oyuncunun elinde malzeme var

      // ��pe at
      KitchenObject.DestroyKitchenObject(player.GetKitchenObject());

      InteractLogicServerRpc();
    } else {
      // oyuncunun elinde malzeme yok
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void InteractLogicServerRpc() {
    InteractLogicClientRpc();
  }

  [ClientRpc]
  private void InteractLogicClientRpc() {
    // ses i�in
    OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
  }
}