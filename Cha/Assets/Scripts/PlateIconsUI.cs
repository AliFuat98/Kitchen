using UnityEngine;

public class PlateIconsUI : MonoBehaviour {

  /// icon'larýn durduðu tabak
  [SerializeField] private PlateKitchenObject plateKitchenObject;

  /// spwan olcak icon
  [SerializeField] private Transform iconTemplate;

  private void Awake() {
    // template'in görüntüsünü kapat
    iconTemplate.gameObject.SetActive(false);
  }

  private void Start() {
    plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
  }

  private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    // öncekileri sil
    foreach (Transform child in transform) {
      if (child == iconTemplate) {
        // template'i silmiyoruz instantiate ederken 1 tanesi lazým
        continue;
      }
      // geri kalanýný sil
      Destroy(child.gameObject);
    }

    // tüm liste kadar icon spawn et her bir yeni malzeme geldiðinde
    foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
      Transform iconTransform = Instantiate(iconTemplate, transform);

      // iconTemplate'in sprite'ýný deðiþtir
      iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
    }
  }
}