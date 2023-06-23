public class TrashCounter : BaseCounter {

  public override void Interact(Player player) {
    if (player.HasKitchenObject()) {
      // oyuncunun elinde malzeme var

      // חצpe at
      player.GetKitchenObject().DestroyItelf();
    } else {
      // oyuncunun elinde malzeme yok
    }
  }
}