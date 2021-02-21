using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour {
  public List<Camera> Cameras;

  private void Start() {
    EnableCamera(0);
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1) && Cameras.Count >= 1) {
      EnableCamera(0);
    } else if (Input.GetKeyDown(KeyCode.Alpha2) && Cameras.Count >= 2) {
      EnableCamera(1);
    } else if (Input.GetKeyDown(KeyCode.Alpha3) && Cameras.Count >= 3) {
      EnableCamera(2);
    } else if (Input.GetKeyDown(KeyCode.Alpha4) && Cameras.Count >= 4) {
      EnableCamera(3);
    } else if (Input.GetKeyDown(KeyCode.Alpha5) && Cameras.Count >= 5) {
      EnableCamera(4);
    } else if (Input.GetKeyDown(KeyCode.Alpha6) && Cameras.Count >= 6) {
      EnableCamera(5);
    } else if (Input.GetKeyDown(KeyCode.Alpha7) && Cameras.Count >= 7) {
      EnableCamera(6);
    } else if (Input.GetKeyDown(KeyCode.Alpha8) && Cameras.Count >= 8) {
      EnableCamera(7);
    } else if (Input.GetKeyDown(KeyCode.Alpha9) && Cameras.Count >= 9) {
      EnableCamera(8);
    } else if (Input.GetKeyDown(KeyCode.Alpha0) && Cameras.Count >= 0) {
      EnableCamera(9);
    }
  }

  private void EnableCamera(int n) {
    Cameras.ForEach(cam => cam.enabled = false);
    Cameras[n].enabled = true;
  }
}