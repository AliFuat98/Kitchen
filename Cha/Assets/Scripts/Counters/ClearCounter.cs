using UnityEngine;

public class ClearCounter : BaseCounter {

  /// bu kutunun üzerinde spawn edeceðimiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun üzeri boþ

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // malzemeyi kutunun üzerine ýþýnla
        player.GetKitchenObject().SetKitchenObjectParent(this);
      } else {
        // oyuncunun eli boþ

        // bir þey yapmamýza gerek yok
      }
    } else {
      // kutunun üzeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // bir þey yapmamýza gerek yok
      } else {
        // oyuncunun eli boþ

        // kutunun üzerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
      }
    }
  }
}