using UnityEngine;

public class CuttingCounter : BaseCounter {

  /// kesildikten sonraki hali için obje
  [SerializeField] private KitchenObjectSO cutKitchenObjectSO;

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

  public override void InteractAlternate(Player player) {
    if (HasKitchenObject()) {
      // kesilmesi gereken malzeme var

      // öncekini sil
      GetKitchenObject().DestroyItelf();

      // yenisini spwan et
      KitchenObject.SpwanKitchenObject(cutKitchenObjectSO, this);
    } else {
      // kutunun üzeri boþ
    }
  }
}