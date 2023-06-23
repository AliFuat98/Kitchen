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

        // ---
      }
    } else {
      // kutunun üzeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
          // oyuncunun elindeki malzeme bir tabak => out parametresiyle ulaþýlabilir

          if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
            // kutunun üzerindeki malzeme tabaða eklenebilir. => eklendi

            // kutunun üzerindekini yok et
            GetKitchenObject().DestroyItelf();
          }
        } else {
          // oyuncu tabak hariç baþka bir þey taþýyor

          if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
            // kutunun üzerinde tabak var

            if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
              // oyuncunun elindeki malzeme tabaða eklenebilir. => eklendi

              // oyuncunun elineki malzemeyi yok et
              player.GetKitchenObject().DestroyItelf();
            } else {
              // oyuncunun elindeki malzeme tabaða eklenmez.

              // ---
            }
          } else {
            // kutunun üzerinde tabak hariç baþka bir þey var

            // ---
          }
        }
      } else {
        // oyuncunun eli boþ

        // kutunun üzerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
      }
    }
  }
}