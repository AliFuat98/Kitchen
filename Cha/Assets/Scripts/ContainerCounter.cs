using System;
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

      OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
  }
}