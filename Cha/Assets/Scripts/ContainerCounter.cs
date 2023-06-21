using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {

  /// oyuncu bir malzeme aldığında gerçekleşecek event
  public event EventHandler OnPlayerGrabbedObject;

  /// bu kutunun üzerinde spawn edeceğimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    // malzemeyi oluştur
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi oyuncuya ver
    kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

    OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
  }
}