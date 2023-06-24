public class DeliveryCounter : BaseCounter {

  public override void Interact(Player player) {
    if (player.HasKitchenObject()) {
      // oyuncunun elinde malzeme var
      if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
        // oyuncunun elinde tabak var

        // sipariþi tamamla
        DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

        // tabaðý yok et
        player.GetKitchenObject().DestroyItelf();
      } else {
        // oyuncunun elinde tabak harici bir malzeme var
      }
    } else {
      // oyuncunun elinde malzeme yok
    }
  }
}