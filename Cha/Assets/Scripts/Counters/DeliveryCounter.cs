public class DeliveryCounter : BaseCounter {
  public static DeliveryCounter Instance { get; private set; }

  private void Awake() {
    Instance = this;
  }

  public override void Interact(Player player) {
    if (player.HasKitchenObject()) {
      // oyuncunun elinde malzeme var
      if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
        // oyuncunun elinde tabak var

        // sipari�i tamamla
        DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

        // taba�� yok et
        player.GetKitchenObject().DestroyItelf();
      } else {
        // oyuncunun elinde tabak harici bir malzeme var
      }
    } else {
      // oyuncunun elinde malzeme yok
    }
  }
}