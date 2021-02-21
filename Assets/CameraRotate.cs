using UnityEngine;

public class CameraRotate : MonoBehaviour {
  public Transform target;
  public float rotateSpeed = 1;

  void LateUpdate() {
    transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);
  }
}
