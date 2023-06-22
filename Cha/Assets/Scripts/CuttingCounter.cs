using UnityEngine;

public class CuttingCounter : BaseCounter {

  /// kesildikten sonra neye dönüþeceðini depolar
  [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun üzeri boþ

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde kesilebilir bir malzeme var

          // malzemeyi kutunun üzerine býrak
          player.GetKitchenObject().SetKitchenObjectParent(this);
        } else {
          // oyuncunun elinde kesilebilir bir malzeme yok

          // ---
        }
      } else {
        // oyuncunun eli boþ

        // ---
      }
    } else {
      // kutunun üzeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // ---
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

      // kesildikten sonra neye dönüþecek BUL
      var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

      if (outputKitchenObjectSO == null) {
        // kesilemez bir malzeme kesimi durdur.
        return;
      }

      // öncekini sil
      GetKitchenObject().DestroyItelf();

      // yenisini spwan et
      KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
    } else {
      // kutunun üzeri boþ
    }
  }

  /// domates verirsen kesilmiþ domates dönecek
  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var cuttingRecipeSO in cuttingRecipeSOArray) {
      if (cuttingRecipeSO.input == inputKitchenObjectSO) {
        return cuttingRecipeSO.output;
      }
    }
    return null;
  }

  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var cuttingRecipeSO in cuttingRecipeSOArray) {
      if (cuttingRecipeSO.input == inputKitchenObjectSO) {
        return true;
      }
    }
    return false;
  }
}