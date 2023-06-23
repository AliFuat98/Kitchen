using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

  /// progres de�i�ti�inde �al��acak event
  public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

  /// animasyonun oynamas� i�in event
  public event EventHandler OnCut;

  /// kesildikten sonra neye d�n��ece�ini depolar
  [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

  private int cuttingProgress;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun �zeri bo�

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde kesilebilir bir malzeme var

          // malzemeyi kutunun �zerine b�rak
          player.GetKitchenObject().SetKitchenObjectParent(this);

          // kesme i�lemi s�re� ba�lat
          cuttingProgress = 0;

          // normalized veri i�in max de�er laz�m tarifin i�inde bu
          var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

          OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
          });
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

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
          // oyuncunun elindeki malzeme bir tabak => out parametresiyle ula��labilir

          if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
            // kutunun �zerindeki malzeme taba�a eklenebilir. => eklendi

            // kutunun �zerindekini yok et
            GetKitchenObject().DestroyItelf();
          }
        } else {
          // oyuncu tabak hari� ba�ka bir �ey ta��yor

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
      }
    }
  }

  /// F tu�una bas�l�nca �al���r kesim
  public override void InteractAlternate(Player player) {
    if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
      // kesilmesi gereken malzeme var  &&  malzeme i�in tarif var kesilebilir

      // bir adet kesme i�lemi ekle
      cuttingProgress++;

      var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
      });

      OnCut?.Invoke(this, EventArgs.Empty);

      if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
        // son kesme i�lemine geldik d�n���m ger�ekle�ebilir

        // kesildikten sonra neye d�n��ecek BUL
        var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        // �ncekini sil
        GetKitchenObject().DestroyItelf();

        // yenisini spwan et
        KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
      }
    } else {
      // kutunun �zeri bo�
    }
  }

  /// gelen malzemenin input oldu�u tarifi ver
  private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var cuttingRecipeSO in cuttingRecipeSOArray) {
      if (cuttingRecipeSO.input == inputKitchenObjectSO) {
        return cuttingRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin kesilmi� halini d�n
  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
    if (cuttingRecipeSO == null) {
      // gelen malzeme tarifte yok
      return null;
    }

    return cuttingRecipeSO.output;
  }

  /// gelen malzemenin tarifi var m�?
  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
    return cuttingRecipeSO != null;
  }
}