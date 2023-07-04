using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

  public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

  public class OnIngredientAddedEventArgs : EventArgs {
    public KitchenObjectSO kitchenObjectSO;
  }

  [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

  private List<KitchenObjectSO> kitchenObjectSOList;

  protected override void Awake() {
    // follow transform null olmamasý için o da awake de çalýþýyor
    base.Awake();
    kitchenObjectSOList = new List<KitchenObjectSO>();
  }

  public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
    if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
      // geçersiz bir malzeme
      return false;
    }

    if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
      // zaten bu malzeme türünden var
      return false;
    }

    AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
    return true;
  }

  [ServerRpc(RequireOwnership = false)]
  private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
    AddIngredientClientRpc(kitchenObjectSOIndex);
  }

  [ClientRpc]
  private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
    KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

    if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
      // zaten bu malzeme türünden var
      return;
    }

    // yeni bir malzeme türü => malzemeyi ekle
    kitchenObjectSOList.Add(kitchenObjectSO);

    OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
      kitchenObjectSO = kitchenObjectSO
    });
  }

  public List<KitchenObjectSO> GetKitchenObjectSOList() {
    return kitchenObjectSOList;
  }
}