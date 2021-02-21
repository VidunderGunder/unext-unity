using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Barracuda;

// Docs:
// https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md#agents

[RequireComponent(typeof(NewCarController))]
public class VehicleAgent : Agent {
  [Header("Dependencies")]
  public RandomEnvironment env;
  public Transform target;
  public ObjectSpawner objectSpawner;

  private Vector3 agentStartPosition;
  private Quaternion agentStartRotation;
  private NewCarController vehicleController;
  private CameraSensorComponent depth;
  private List<float> rewards = new List<float>();

  [Range(-1f, 1f)] private float motor = 0;
  [Range(-1f, 1f)] private float steering = 0;
  [Range(0, 1f)] private int brake = 0;

  private void Awake() {
    vehicleController = GetComponent<NewCarController>();
    depth = GetComponent<CameraSensorComponent>();
  }

  public override void Initialize() {
    // 
  }

  private void Start() {
    agentStartPosition = transform.position;
    agentStartRotation = transform.rotation;
  }

  public override void OnEpisodeBegin() {
    Respawn();
  }

  public override void CollectObservations(VectorSensor sensor) {
    // Visual input (depth) is handled automagically by Unity
    // sensor.AddObservation()
  }

  public override void OnActionReceived(ActionBuffers actionBuffers) {
    var continuousActions = actionBuffers.ContinuousActions; // Range is [-1f, 1f]
    var discreteActions = actionBuffers.DiscreteActions; // Range is [0, discreteBranchSize]
    int c = -1; int d = -1;

    motor = continuousActions[++c];
    steering = continuousActions[++c];
    brake = discreteActions[++d];

    bool agentBelowGround = transform.position.y < agentStartPosition.y - 10;
    bool agentOutsideArea = Vector3.Distance(
      transform.position,
      agentStartPosition
    ) > Mathf.Max(
      env.length / 2,
      env.width / 2
    ) * 1.25f;
    bool episodeShouldEnd = agentBelowGround & agentOutsideArea;

    if (episodeShouldEnd) {
      EndEpisode();
    }

    AddReward(-0.001f); // Existential penalty
  }

  public override void Heuristic(in ActionBuffers actionsOut) {
    var continousActions = actionsOut.ContinuousActions;
    var discreteActions = actionsOut.DiscreteActions;
    int c = -1; int d = -1;

    continousActions[++c] = Input.GetAxis("Horizontal"); // Motor
    continousActions[++c] = Input.GetAxis("Vertical"); // Steering
    discreteActions[++d] = Input.GetAxis("Jump") > 0 ? 1 : 0; // Brake
  }

  private void Respawn() {
    transform.position = agentStartPosition;
    transform.rotation = agentStartRotation;
    GetComponent<Rigidbody>().velocity = Vector3.zero;
    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    objectSpawner.Respawn();
  }

  private void OnTriggerStay(Collider other) {
    if (other.tag == "Target") {
      if (GetComponent<Rigidbody>().velocity.magnitude < 0.01f) {
        Debug.Log("Success!");
        AddReward(1f);
        rewards.Add(GetCumulativeReward());
        EndEpisode();
      }
    }
  }

  private void Update() {
    // Debug.Log(rewards);
    if (rewards.Count >= 3) {
      Debug.Log("Last rewards");
      Debug.Log(rewards[rewards.Count - 1]);
      Debug.Log(rewards[rewards.Count - 2]);
      Debug.Log(rewards[rewards.Count - 3]);
    }
  }

  private void FixedUpdate() {
    vehicleController.Move(motor, steering, steering, (float)brake);
  }

  void SpawnTarget(Transform prefab, Vector3 spawnPosition) {
    target = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
  }

  public void TouchedTarget() {
    AddReward(1f);
  }
}

