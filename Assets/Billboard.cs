using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour {
  public Transform cam;

  void LateUpdate() {
    transform.LookAt(transform.position + cam.forward);
  }
}
