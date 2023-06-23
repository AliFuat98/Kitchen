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

        // ---
      }
    } else {
      // kutunun �zeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
          // oyuncunun elindeki malzeme bir tabak => out parametresiyle ula��labilir

          if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
            // kutunun �zerindeki malzeme taba�a eklenebilir. => eklendi

            // kutunun �zerindekini yok et
            GetKitchenObject().DestroyItelf();
          }
        } else {
          // oyuncu tabak hari� ba�ka bir �ey ta��yor

          if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
            // kutunun �zerinde tabak var

            if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
              // oyuncunun elindeki malzeme taba�a eklenebilir. => eklendi

              // oyuncunun elineki malzemeyi yok et
              player.GetKitchenObject().DestroyItelf();
            } else {
              // oyuncunun elindeki malzeme taba�a eklenmez.

              // ---
            }
          } else {
            // kutunun �zerinde tabak hari� ba�ka bir �ey var

            // ---
          }
        }
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
      }
    }
  }
}