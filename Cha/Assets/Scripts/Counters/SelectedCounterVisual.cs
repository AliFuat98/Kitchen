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
    Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
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