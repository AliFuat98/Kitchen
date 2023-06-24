using UnityEngine.SceneManagement;

public static class Loader {

  public enum Scene {
    MainMenu,
    Game,
    Loading,
  }

  private static Scene targetScene;

  public static void Load(Scene targetScene) {
    Loader.targetScene = targetScene;

    SceneManager.LoadScene($"{Scene.Loading}Scene");
  }

  public static void LoaderCallBack() {
    SceneManager.LoadScene($"{targetScene}Scene");
  }
}