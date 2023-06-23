using UnityEngine;

public class ClearCounter : BaseCounter {

  /// bu kutunun �zerinde spawn edece�imiz scriptable obje (malzeme)
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

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
}