using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

  /// singleton
  public static DeliveryManager Instance { get; private set; }

  /// spwan edilebilecek sipari� listesi
  [SerializeField] private RecipeListSO recipeSOList;

  /// bekleyen sipari�ler
  private List<RecipeSO> waitingRecipeSOList;

  private float spawnRecipeTimer;
  private float spawnRecipeTimerMax = 4f;

  private int waitingRecipeMax = 4;

  private void Awake() {
    if (Instance != null) {
      Debug.Log("birden fazla delivery manager");
    }
    Instance = this;

    waitingRecipeSOList = new List<RecipeSO>();
  }

  private void Update() {
    spawnRecipeTimer -= Time.deltaTime;
    if (spawnRecipeTimer < 0f) {
      // yeni bir sipari� spawn vakti

      // s�reyi ba�a al
      spawnRecipeTimer = spawnRecipeTimerMax;

      if (waitingRecipeSOList.Count < waitingRecipeMax) {
        // yeni sipari� i�in yer var

        // listeden rastgele bir sipari� se�
        RecipeSO waitingRecipeSO = recipeSOList.recipeSOList[Random.Range(0, recipeSOList.recipeSOList.Count)];

        // listeye ekle
        waitingRecipeSOList.Add(waitingRecipeSO);

        Debug.Log(waitingRecipeSO.recipeName);
      }
    }
  }

  public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
    // taba��n �zerindeki malzemeleri tutan liste
    var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

    // bekleyen bir sipari�le tabak uyu�uyor mu hepsine tek tek bak�yoruz
    for (int i = 0; i < waitingRecipeSOList.Count; i++) {
      RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

      if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObjectSOList.Count) {
        // ayn� say�da malzeme var

        bool recipeMatch = true;
        foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOList) {
          // sipari�teki �r�nlere bak�yoruz

          if (!plateKitchenObjectSOList.Contains(recipeKitchenObjectSO)) {
            // tabakta eksik var
            recipeMatch = false;
            break;
          } else {
            // tabakta var sipari�teki di�er �r�nlere bakabiliriz

            // ---
          }
        }

        if (recipeMatch) {
          // tabaktaki malzeme ile e�le�en bir sipari� var => sipari� do�ru haz�rlanm��
          Debug.Log("sipari� do�ru");

          // bekleyen sipari� listesinden kald�r
          waitingRecipeSOList.RemoveAt(i);

          return;
        } else {
          //  gezdi�imiz s�radaki sipari� e�le�me sa�lamad� => sonrakilere bakmaya devam et
        }
      } else {
        // gezdi�imiz s�radaki sipari�te malzeme say�s� tutmuyor  => sonrakilere bakmaya devam et
      }
    }

    // tabaktaki malzeme ile e�le�en bir sipari� yok => sipari� eksik haz�rlanm��
    Debug.Log("sipari� hatal�");
  }
}