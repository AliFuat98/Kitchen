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

        // sipariþi tamamla
        DeliveryManager.Instance.DeliverRecipe(plateKitchenObject,player);

        // tabaðý yok et
        KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
      } else {
        // oyuncunun elinde tabak harici bir malzeme var
      }
    } else {
      // oyuncunun elinde malzeme yok
    }
  }
}