using UnityEngine;

public class ClearCounter : BaseCounter {

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
  }
}