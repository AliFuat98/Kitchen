using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {

  /// recipe template'leri tutacak Game object
  [SerializeField] private Transform container;

  [SerializeField] private Transform recipeTemplate;

  private void Awake() {
    recipeTemplate.gameObject.SetActive(false);
  }

  private void Start() {
    DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
    DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;

    UpdateVisual();
  }

  private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e) {
    UpdateVisual();
  }

  private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e) {
    UpdateVisual();
  }

  private void UpdateVisual() {
    foreach (Transform child in container) {
      if (child == recipeTemplate) {
        continue;
      }

      Destroy(child.gameObject);
    }
    foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()) {
      Transform recipeTemplateTransform = Instantiate(recipeTemplate, container);
      recipeTemplateTransform.gameObject.SetActive(true);

      recipeTemplateTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
    }
  }
}