using UnityEngine;

public class CuttingCounter : BaseCounter {

  /// kesildikten sonra neye d�n��ece�ini depolar
  [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun �zeri bo�

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde kesilebilir bir malzeme var

          // malzemeyi kutunun �zerine b�rak
          player.GetKitchenObject().SetKitchenObjectParent(this);
        } else {
          // oyuncunun elinde kesilebilir bir malzeme yok

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // ---
      }
    } else {
      // kutunun �zeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        // ---
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

      // kesildikten sonra neye d�n��ecek BUL
      var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

      if (outputKitchenObjectSO == null) {
        // kesilemez bir malzeme kesimi durdur.
        return;
      }

      // �ncekini sil
      GetKitchenObject().DestroyItelf();

      // yenisini spwan et
      KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
    } else {
      // kutunun �zeri bo�
    }
  }

  /// domates verirsen kesilmi� domates d�necek
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