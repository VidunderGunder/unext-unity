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
  public List<float> rewards = new List<float>();

  [System.NonSerialized] public float minCumulativeReward = -2f;
  [System.NonSerialized] public float maxCumulativeReward = 3f;

  [SerializeField] private RandomEnvironment env;
  [SerializeField] private Transform target;
  [SerializeField] private ObjectSpawner objectSpawner;
  [SerializeField] private NewCarController vehicleController;
  [SerializeField] private Rigidbody agentRigidbody;
  [SerializeField] private CameraSensorComponent depth;
  [SerializeField] private bool training = true;

  [System.NonSerialized] public Vector3 agentStartPosition;
  [System.NonSerialized] public Quaternion agentStartRotation;

  private float maxDistanceFromStart;
  private float maxDistanceFromStartSq;
  private bool atTarget;
  private int timesAtTarget;
  private bool hasStopped;
  private float closestDistanceSq;
  private float initialDistanceSq;

  private float maxSingleCollisionPenalty = 1.5f;
  private float maxCumulativeCollisionPenalty = 2f;
  private float cumulativeCollisionPenalty = 0;

  [Range(-1f, 1f)] public float motor = 0;
  [Range(-1f, 1f)] public float steering = 0;
  [Range(0, 1f)] public int brake = 0;

  [Observable]
  float targetDistanceSq {
    get {
      return target != null ? (transform.position - target.position).sqrMagnitude : 0;
    }
  }

  private void Awake() {
    if (vehicleController == null) vehicleController = GetComponent<NewCarController>();
    if (agentRigidbody == null) agentRigidbody = GetComponent<Rigidbody>();
    if (depth == null) depth = GetComponent<CameraSensorComponent>();
    if (target == null) target = transform.parent.Find("Target");
    agentStartPosition = transform.position;
    agentStartRotation = transform.rotation;

    if (!training) {
      return;
    }

    if (env == null) env = GetComponentInParent<RandomEnvironment>();
    if (objectSpawner == null) objectSpawner = transform.parent.Find("Object Spawner").GetComponent<ObjectSpawner>();

    transform.parent = env.transform;
    transform.localPosition = Vector3.zero;

    maxDistanceFromStart = Mathf.Max(
      env.length / 2,
      env.width / 2
    ) * 1.25f;
    maxDistanceFromStartSq = maxDistanceFromStart * maxDistanceFromStart;
  }

  public override void OnEpisodeBegin() {
    if (!training) {
      return;
    }

    cumulativeCollisionPenalty = 0;
    timesAtTarget = 0;
    Respawn();
    closestDistanceSq = targetDistanceSq;
    initialDistanceSq = targetDistanceSq;
  }

  public override void CollectObservations(VectorSensor sensor) {
    if (target == null) {
      return;
    }

    Vector3 relativePosition = target.transform.position - transform.position;

    relativePosition = Quaternion.Inverse(transform.rotation) * relativePosition;

    sensor.AddObservation(relativePosition.x);
    sensor.AddObservation(relativePosition.z);

    if (!training) {
      return;
    }

    if (StepCount == MaxStep) {
      rewards.Add(GetCumulativeReward());
    }
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

    if (!training) {
      return;
    }

    AddReward((-1f / MaxStep) * (hasStopped & !atTarget ? 1f + env.difficulty : 0.1f)); // Existential penalty
  }

  public void Respawn() {
    transform.position = agentStartPosition;
    transform.rotation = agentStartRotation;
    agentRigidbody.velocity = Vector3.zero;
    agentRigidbody.angularVelocity = Vector3.zero;

    if (!training) {
      return;
    }

    objectSpawner.Respawn();
  }

  private void OnCollisionEnter(Collision collision) {
    if (!training) {
      return;
    }

    if (
      collision.collider.name != "Mesh Generator"
    ) {
      float collisionPenalty = collision.relativeVelocity.sqrMagnitude * 0.025f;
      collisionPenalty = collisionPenalty > maxSingleCollisionPenalty ? maxSingleCollisionPenalty : collisionPenalty;
      cumulativeCollisionPenalty += collisionPenalty;
      if (cumulativeCollisionPenalty > maxCumulativeCollisionPenalty) {
        collisionPenalty = maxCumulativeCollisionPenalty - cumulativeCollisionPenalty;
        cumulativeCollisionPenalty = maxCumulativeCollisionPenalty;
      }
      AddReward(-collisionPenalty);
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Target")) {
      atTarget = true;

      if (!training) {
        return;
      }

      ++timesAtTarget;
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.CompareTag("Target")) {
      atTarget = false;
    }
  }

  private void FixedUpdate() {
    hasStopped = agentRigidbody.velocity.sqrMagnitude < 0.0002f;
    bool success = hasStopped & atTarget;

    if (!training) {
      if (!success) {
        vehicleController.Move(motor, steering, steering, (float)brake);
      }
      return;
    }


    bool agentBelowGround = transform.position.y < agentStartPosition.y - 10f;
    bool agentOutsideArea = (transform.position - agentStartPosition).sqrMagnitude > maxDistanceFromStartSq;

    bool targetBelowGround = target.transform.position.y < agentStartPosition.y - 10f;
    bool targetOutsideArea = (target.transform.position - agentStartPosition).sqrMagnitude > maxDistanceFromStartSq;

    bool error =
      agentBelowGround |
      agentOutsideArea |
      targetBelowGround |
      targetOutsideArea;

    bool episodeShouldEnd = error | success;

    if (!atTarget & targetDistanceSq < closestDistanceSq) {
      AddReward((closestDistanceSq - targetDistanceSq) / initialDistanceSq);
      closestDistanceSq = targetDistanceSq;
    }

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
