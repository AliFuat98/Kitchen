using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

  /// <summary>
  /// bu deðiþken kutuyu kapsayan en dýþtaki gameObject'tir
  /// player'daki Player_OnSelectedCounterChanged eventine sub olduktan sonra deðiþen kutu biz miyiz onun kontrolü için
  /// </summary>
  [SerializeField] private ClearCounter clearCounter;

  /// <summary>
  /// seçiliysek tipimiz deðiþsin die aç kapa yapacaðýmz obje
  /// Selected gameObjesinin altýndaki visual
  /// </summary>
  [SerializeField] private GameObject visualGameObject;

  private void Start() {
    Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
  }

  private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
    if (e.selectedCounter == clearCounter) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    visualGameObject.SetActive(true);
  }

  private void Hide() {
    visualGameObject.SetActive(false);
  }
}