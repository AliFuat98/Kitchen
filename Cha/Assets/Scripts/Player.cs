using UnityEngine;

public class Player : MonoBehaviour {
  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private GameInput gameInput;

  private bool isWalking;

  public bool IsWalking() {
    return isWalking;
  }

  private void Update() {
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float moveDistance = Time.deltaTime * moveSpeed;
    float playerRadius = .6f;
    float playerHeight = 2f;
    var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    // X veya Z de engel Var
    if (!canMove) {
      Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
      canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

      // X eksenin de engel yok Z ekseninde Var
      if (canMove) {
        moveDir = moveDirX;

        // X ekseninde engel var Z ekseninde yok
      } else {
        Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

        // X engel var Z yok
        if (canMove) {
          moveDir = moveDirZ;
        } else {
          // 2 tarafta engelli
          moveDir = Vector3.zero;
        }
      }
    }

    // X ve Z eksenlerinde engel yok
    if (canMove) {
      transform.position += moveDir * moveDistance;
    } 

    isWalking = moveDir != Vector3.zero;

    float rotationSpeed = 10f;
    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
  }
}