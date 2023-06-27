using System;
using System.Collections.Generic;
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

    // yeni bir malzeme türü => malzemeyi ekle
    kitchenObjectSOList.Add(kitchenObjectSO);

    OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
      kitchenObjectSO = kitchenObjectSO
    });
    return true;
  }

  public List<KitchenObjectSO> GetKitchenObjectSOList() {
    return kitchenObjectSOList;
  }
}