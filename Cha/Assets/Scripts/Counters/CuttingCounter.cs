using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

  /// ses i�in �al��acak event
  public static event EventHandler OnAnyCut;

  /// scene de�i�ti�inde temizlik yap�lmas� gerekiyor
  public static new void ResetStaticData() {
    OnAnyCut = null;
  }

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
          KitchenObject kitchenObject = player.GetKitchenObject();
          kitchenObject.SetKitchenObjectParent(this);

          InteractLogicPlaceObjectOnCounterServerRpc();
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
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
          }
        } else {
          // oyuncu tabak hari� ba�ka bir �ey ta��yor

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);
        InteractLogicPlaceObjectOnCounterServerRpc();
      }
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void InteractLogicPlaceObjectOnCounterServerRpc() {
    InteractLogicPlaceObjectOnCounterClientRpc();
  }

  [ClientRpc]
  private void InteractLogicPlaceObjectOnCounterClientRpc() {
    // kesme i�lemi s�re� ba�lat
    cuttingProgress = 0;

    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      progressNormalized = 0f
    });
  }

  /// F tu�una bas�l�nca �al���r kesim
  public override void InteractAlternate(Player player) {
    if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
      // kesilmesi gereken malzeme var  &&  malzeme i�in tarif var kesilebilir

      CutObjectServerRpc();
      TestCuttingProgressDoneServerRpc();
    } else {
      // kutunun �zeri bo�
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void CutObjectServerRpc() {
    if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
      // kesilmesi gereken malzeme var  &&  malzeme i�in tarif var kesilebilir
      CutObjectClientRpc();
    }
  }

  [ClientRpc]
  private void CutObjectClientRpc() {
    // bir adet kesme i�lemi ekle
    cuttingProgress++;

    var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax,
    });

    OnCut?.Invoke(this, EventArgs.Empty);
    OnAnyCut?.Invoke(this, EventArgs.Empty);
  }

  [ServerRpc(RequireOwnership = false)]
  private void TestCuttingProgressDoneServerRpc() {
    if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
      // kesilmesi gereken malzeme var  &&  malzeme i�in tarif var kesilebilir
      var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

      if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
        // son kesme i�lemine geldik d�n���m ger�ekle�ebilir

        // kesildikten sonra neye d�n��ecek BUL
        var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

        // �ncekini sil
        KitchenObject.DestroyKitchenObject(GetKitchenObject());

        // yenisini spwan et
        KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
      }
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