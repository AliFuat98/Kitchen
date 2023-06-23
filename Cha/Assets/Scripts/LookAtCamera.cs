using UnityEngine;

public class LookAtCamera : MonoBehaviour {

  private enum Mode {
    LookAt,
    LookAtInverted,
    CameraForward,
    CameraBackwardInverted,
  }

  [SerializeField] private Mode mode;

  private void LateUpdate() {
    switch (mode) {
      case Mode.LookAt:
        // camerata doðru dön
        transform.LookAt(Camera.main.transform);
        break;

      case Mode.LookAtInverted:

        // kameraya arkaný dön
        Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + dirFromCamera);
        break;

      case Mode.CameraForward:
        transform.forward = Camera.main.transform.forward;
        break;

      case Mode.CameraBackwardInverted:
        transform.forward = -Camera.main.transform.forward;
        break;

      default:
        break;
    }
  }
}