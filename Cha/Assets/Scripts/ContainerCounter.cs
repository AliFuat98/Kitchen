using System;
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

      OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
  }
}