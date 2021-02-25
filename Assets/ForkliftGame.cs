using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftGame : MonoBehaviour {
  public VehicleAgent agent;
  public Transform target;
  private string[] safeNames = {
    "Asphalt",
    "Grass 1",
    "Grass 2",
    "Grass 3",
    "Grass 4",
    "Grass 5",
    "Grass 6",
    "Grass 7",
    "Grass 8",
  };

  public void Restart() {
    // Agent
    agent.Respawn();

    // Target
    target.localPosition = new Vector3(-20f, 1f, 8f);
    target.rotation = Quaternion.Euler(Vector3.zero);
    Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
    targetRigidbody.velocity = Vector3.zero;
    targetRigidbody.angularVelocity = Vector3.zero;
  }

  void Awake() {
    if (target == null) target = transform.Find("Forklift Target");
    if (agent == null) agent = transform.Find("Forklift Agent Inference").GetComponent<VehicleAgent>();
  }

  private void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Clicked();
    }
  }

  void Clicked() {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    RaycastHit hit = new RaycastHit();

    if (Physics.Raycast(ray, out hit)) {
      Collider[] colliders = Physics.OverlapSphere(hit.point, 2f);
      foreach (Collider col in colliders) {
        bool safe = false;

        Debug.Log(col.gameObject.name);
        foreach (string safeName in safeNames) {
          if (col.gameObject.name.Equals(safeName)) safe = true;
        }

        if (!safe) return;
      }
      target.position = hit.point + new Vector3(0, 1f, 0);
    }
  }
}
