using UnityEngine;

public class CuttingCounter : BaseCounter {

  /// kesildikten sonraki hali i�in obje
  [SerializeField] private KitchenObjectSO cutKitchenObjectSO;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun �zeri bo�

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // malzemeyi kutunun �zerine ���nla
        player.GetKitchenObject().SetKitchenObjectParent(this);
      } else {
        // oyuncunun eli bo�

        // bir �ey yapmam�za gerek yok
      }
    } else {
      // kutunun �zeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // bir �ey yapmam�za gerek yok
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
      }
    }
  }

  public override void InteractAlternate(Player player) {
    if (HasKitchenObject()) {
      // kesilmesi gereken malzeme var

      // �ncekini sil
      GetKitchenObject().DestroyItelf();

      // yenisini spwan et
      KitchenObject.SpwanKitchenObject(cutKitchenObjectSO, this);
    } else {
      // kutunun �zeri bo�
    }
  }
}