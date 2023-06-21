using System;
using UnityEngine;

public class ContainerCounter : BaseCounter {

  /// oyuncu bir malzeme ald���nda ger�ekle�ecek event
  public event EventHandler OnPlayerGrabbedObject;

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    // malzemeyi olu�tur
    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    // malzemeyi oyuncuya ver
    kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

    OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
  }
}