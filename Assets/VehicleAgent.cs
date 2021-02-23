using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using Unity.MLAgents.Actuators;
// using Unity.Barracuda;

// Docs:
// https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#agents

public class VehicleAgent : Agent {
  public RandomEnvironment env;
  public Transform target;
  public ObjectSpawner objectSpawner;
  public List<float> rewards = new List<float>();
  public Vector3 agentStartPosition;
  public Quaternion agentStartRotation;
  public NewCarController vehicleController;
  public CameraSensorComponent depth;

  public bool atTarget;
  public bool hasStopped;

  [Range(-1f, 1f)] public float motor = 0;
  [Range(-1f, 1f)] public float steering = 0;
  [Range(0, 1f)] public int brake = 0;

  [Observable(numStackedObservations: 3)]
  float targetDistance {
    get {
      return Vector3.Distance(transform.position, target.position);
    }
  }

  [Observable(numStackedObservations: 3)]
  float targetAngle {
    get {
      return Quaternion.Angle(transform.rotation, target.rotation);
    }
  }

  private void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();
    if (target == null) target = transform.parent.Find("Target");
    if (objectSpawner == null) objectSpawner = transform.parent.Find("Object Spawner").GetComponent<ObjectSpawner>();
    if (vehicleController == null) vehicleController = GetComponent<NewCarController>();
    if (depth == null) depth = GetComponent<CameraSensorComponent>();

    transform.parent = env.transform;
    transform.localPosition = Vector3.zero;
    agentStartPosition = transform.position;
    agentStartRotation = transform.rotation;
  }

  // // public override void Initialize() {
  // //   // 
  // // }

  public override void OnEpisodeBegin() {
    Respawn();
  }

  public override void Heuristic(in ActionBuffers actionsOut) {
    var continousActions = actionsOut.ContinuousActions;
    var discreteActions = actionsOut.DiscreteActions;
    int c = -1; int d = -1;

    continousActions[++c] = Input.GetAxis("Horizontal"); // Motor
    continousActions[++c] = Input.GetAxis("Vertical"); // Steering
    discreteActions[++d] = Input.GetAxis("Jump") > 0 ? 1 : 0; // Brake
  }

  public override void OnActionReceived(ActionBuffers actionBuffers) {
    var continuousActions = actionBuffers.ContinuousActions; // Range is [-1f, 1f]
    var discreteActions = actionBuffers.DiscreteActions; // Range is [0, discreteBranchSize]
    int c = -1; int d = -1;

    motor = continuousActions[++c];
    steering = continuousActions[++c];
    brake = discreteActions[++d];

    AddReward(-0.001f); // Existential penalty
  }

  private void Respawn() {
    transform.position = agentStartPosition;
    transform.rotation = agentStartRotation;
    GetComponent<Rigidbody>().velocity = Vector3.zero;
    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    objectSpawner.Respawn();
  }


  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Target")) {
      atTarget = true;
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.CompareTag("Target")) {
      atTarget = false;
    }
  }

  // private void Update() {
  //   // Debug.Log(rewards);
  //   // if (rewards.Count >= 3) {
  //   //   Debug.Log("Last rewards");
  //   //   Debug.Log(rewards[rewards.Count - 1]);
  //   //   Debug.Log(rewards[rewards.Count - 2]);
  //   //   Debug.Log(rewards[rewards.Count - 3]);
  //   // }
  // }

  private void FixedUpdate() {
    vehicleController.Move(motor, steering, steering, (float)brake);

    bool agentBelowGround = transform.position.y < agentStartPosition.y - 10;
    bool agentOutsideArea = Vector3.Distance(
      transform.position,
      agentStartPosition
    ) > Mathf.Max(
      env.length / 2,
      env.width / 2
    ) * 1.25f;
    bool error = agentBelowGround | agentOutsideArea;

    hasStopped = GetComponent<Rigidbody>().velocity.magnitude < 0.01f;
    bool success = hasStopped & atTarget;

    bool episodeShouldEnd = error | success;

    if (episodeShouldEnd) {
      AddReward(success ? 1f : 0);
      rewards.Add(GetCumulativeReward());
      EndEpisode();
    }
  }

  void SpawnTarget(Transform prefab, Vector3 spawnPosition) {
    target = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
  }
}
