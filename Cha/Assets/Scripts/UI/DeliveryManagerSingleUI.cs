using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour {
  [SerializeField] private TextMeshProUGUI recipeNameText;

  [SerializeField] private Transform IconContainer;
  [SerializeField] private Transform IconTemplate;

  private void Awake() {
    IconTemplate.gameObject.SetActive(false);
  }

  public void SetRecipeSO(RecipeSO recipeSO) {
    recipeNameText.text = recipeSO.recipeName;

    // template hariç diðerlerini sil
    foreach (Transform child in IconContainer) {
      if (child == IconTemplate) {
        continue;
      }

      Destroy(child.gameObject);
    }

    foreach (KitchenObjectSO kitchenObjectSO in recipeSO.KitchenObjectSOList) {
      Transform iconTemplateTransform = Instantiate(IconTemplate, IconContainer);
      iconTemplateTransform.gameObject.SetActive(true);

      iconTemplateTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
    }
  }
}