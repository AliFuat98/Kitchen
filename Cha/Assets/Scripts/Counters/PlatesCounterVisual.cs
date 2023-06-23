using System.Linq;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {
  [SerializeField] private PlatesCounter platesCounter;

  [SerializeField] private Transform plateVisualPrefab;
  [SerializeField] private GameObject[] PlatesGameObjectArray;

  private void Start() {
    platesCounter.OnPlateSpawn += PlatesCounter_OnPlateSpawn;
    platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
  }

  private void PlatesCounter_OnPlateRemoved(object sender, PlatesCounter.OnPlateSpawnEventArgs e) {
    PlatesGameObjectArray.ElementAt(e.platesSpawnedAmount).SetActive(false);
  }

  private void PlatesCounter_OnPlateSpawn(object sender, PlatesCounter.OnPlateSpawnEventArgs e) {
    PlatesGameObjectArray.ElementAt(e.platesSpawnedAmount).SetActive(true);
  }
}