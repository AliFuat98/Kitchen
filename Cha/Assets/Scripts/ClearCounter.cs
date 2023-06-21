using UnityEngine;

public class ClearCounter : BaseCounter {

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
  }
}