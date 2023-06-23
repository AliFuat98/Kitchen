using UnityEngine;

public class PlateIconsUI : MonoBehaviour {

  /// icon'lar�n durdu�u tabak
  [SerializeField] private PlateKitchenObject plateKitchenObject;

  /// spwan olcak icon
  [SerializeField] private Transform iconTemplate;

  private void Awake() {
    // template'in g�r�nt�s�n� kapat
    iconTemplate.gameObject.SetActive(false);
  }

  private void Start() {
    plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
  }

  private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    // �ncekileri sil
    foreach (Transform child in transform) {
      if (child == iconTemplate) {
        // template'i silmiyoruz instantiate ederken 1 tanesi laz�m
        continue;
      }
      // geri kalan�n� sil
      Destroy(child.gameObject);
    }

    // t�m liste kadar icon spawn et her bir yeni malzeme geldi�inde
    foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
      Transform iconTransform = Instantiate(iconTemplate, transform);

      // iconTemplate'in sprite'�n� de�i�tir
      iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
    }
  }
}