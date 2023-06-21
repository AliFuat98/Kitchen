using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {

  /// oyuncu bir malzeme aldýðýnda gerçekleþecek event
  public event EventHandler OnPlayerGrabbedObject;

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    // malzemeyi oluþtur
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi oyuncuya ver
    kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

    OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
  }
}