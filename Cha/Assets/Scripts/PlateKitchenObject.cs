using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
  [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

  private List<KitchenObjectSO> kitchenObjectSOList;

  private void Awake() {
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
    return true;
  }
}