using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

  /// <summary>
  /// bu deðiþken kutuyu kapsayan en dýþtaki gameObject'tir
  /// player'daki Player_OnSelectedCounterChanged eventine sub olduktan sonra deðiþen kutu biz miyiz onun kontrolü için
  /// </summary>
  [SerializeField] private BaseCounter baseCounter;

  /// <summary>
  /// seçiliysek tipimiz deðiþsin die aç kapa yapacaðýmz obje
  /// Selected gameObjesinin altýndaki visual
  /// </summary>
  [SerializeField] private GameObject[] visualGameObjectArray;

  private void Start() {
    if (Player.LocalInstance != null) {
      // instance varsa direk event'e subscribe ol
      Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    } else {
      // instance daha oluþmadýysa

      Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
    }
  }

  private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e) {
    if (Player.LocalInstance != null) {
      // bu event birden fazla kez çalýþtýðý için bir sürü subscription'ýmýz olmamasý için
      // bir ade çýkar tak yapýyoruz burda
      Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
      Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }
  }

  private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
    if (e.selectedCounter == baseCounter) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    foreach (var visualGameObject in visualGameObjectArray) {
      visualGameObject.SetActive(true);
    }
  }

  private void Hide() {
    foreach (var visualGameObject in visualGameObjectArray) {
      visualGameObject.SetActive(false);
    }
  }
}