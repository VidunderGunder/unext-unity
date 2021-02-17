using UnityEngine;

public class camera : MonoBehaviour {
  public Transform target;
  public float rotateSpeed;

  void LateUpdate() {
    transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);
  }
}
