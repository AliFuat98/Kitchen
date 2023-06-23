using System;
using UnityEngine;

public class CuttingCounter : BaseCounter {

  /// progres deðiþtiðinde çalýþacak event
  public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

  public class OnProgressChangedEventArgs : EventArgs {
    public float progressNormalized;
  }

  /// animasyonun oynamasý için event
  public event EventHandler OnCut;

  /// kesildikten sonra neye dönüþeceðini depolar
  [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

  private int cuttingProgress;

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun üzeri boþ

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde kesilebilir bir malzeme var

          // malzemeyi kutunun üzerine býrak
          player.GetKitchenObject().SetKitchenObjectParent(this);

          // kesme iþlemi süreç baþlat
          cuttingProgress = 0;

          // normalized veri için max deðer lazým tarifin içinde bu
          var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

          OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
          });
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

  /// F tuþuna basýlýnca çalýþýr kesim
  public override void InteractAlternate(Player player) {
    if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
      // kesilmesi gereken malzeme var  &&  malzeme için tarif var kesilebilir

      // bir adet kesme iþlemi ekle
      cuttingProgress++;

      var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

      OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs {
        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
      });

      OnCut?.Invoke(this, EventArgs.Empty);

      if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
        // son kesme iþlemine geldik dönüþüm gerçekleþebilir

        // kesildikten sonra neye dönüþecek BUL
        var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        // öncekini sil
        GetKitchenObject().DestroyItelf();

        // yenisini spwan et
        KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
      }
    } else {
      // kutunun üzeri boþ
    }
  }

  /// gelen malzemenin input olduðu tarifi ver
  private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var cuttingRecipeSO in cuttingRecipeSOArray) {
      if (cuttingRecipeSO.input == inputKitchenObjectSO) {
        return cuttingRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin kesilmiþ halini dön
  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
    if (cuttingRecipeSO == null) {
      // gelen malzeme tarifte yok
      return null;
    }

    return cuttingRecipeSO.output;
  }

  /// gelen malzemenin tarifi var mý?
  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
    return cuttingRecipeSO != null;
  }
}