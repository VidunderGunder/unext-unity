using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
  // https://docs.unity3d.com/2021.1/Documentation/ScriptReference/Resources.Load.html
  private RandomEnvironment env;
  private GameObject target;
  private Rigidbody targetRigigbody;
  private Transform targetArea;
  string[] targetCollisionTags = { "Vehicle", "RandomObject" };
  string[] randomCollisionTags = { "Vehicle", "Target" };

  [Header("Random Primitive Objects")]
  public int maxAmount = 5;
  public float minScale = 1f;
  public float maxScale = 15f;

  [Range(0, 1)] private float outerSafeArea = 0.25f;
  private List<GameObject> spawnedObjects = new List<GameObject>();
  // private string respawnState = "wait"; // wait, reset, target, random

  void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();
    if (target == null) target = transform.parent.Find("Target").gameObject;
    if (targetRigigbody == null) targetRigigbody = target.GetComponent<Rigidbody>();
    if (targetArea == null) targetArea = target.transform.Find("Area");
  }

  public void Respawn() {
    // Debug.Log("- - - - -");
    // Debug.Log("RESPAWN");
    // Debug.Log("- - - - -");
    DestroySpawnedObjects();
    SpawnTarget();
    for (int i = 0; i < maxAmount * env.difficulty; i++) {
      SpawnRandomPrimitive();
    }
  }

  // public void Respawn() {
  //   respawnState = "reset";
  // }

  // private void RespawnRoutines() {
  //   switch (respawnState) {
  //     case "wait":
  //       break;
  //     case "reset":
  //       DestroySpawnedObjects();
  //       respawnState = "target";
  //       break;
  //     case "target":
  //       SpawnTarget();
  //       respawnState = "random";
  //       break;
  //     case "random":
  //       for (int i = 0; i < maxAmount * env.difficulty; i++) {
  //         SpawnRandomPrimitive();
  //       }
  //       respawnState = "wait";
  //       break;
  //   }
  // }

  // void FixedUpdate() {
  //   RespawnRoutines();
  // }

  bool SpawnTarget(int retries = 1000) {
    for (int i = 0; i < retries; i++) {
      target.transform.localPosition = GetRandomPosition(
        env.width * env.difficulty,
        0,
        env.length * env.difficulty * .5f + (env.difficulty < 0.5f ? 0 : env.length * (env.difficulty - .5f) * .5f)
      ) + new Vector3(
        0,
        0,
        4.9375f + (env.difficulty < 0.5f ? env.difficulty : (1f - env.difficulty)) * env.length * 0.2f
      );

      target.transform.rotation = Quaternion.Euler(Vector3.zero);
      targetRigigbody.velocity = Vector3.zero;
      targetRigigbody.angularVelocity = Vector3.zero;

      if (!Collision(targetArea, targetCollisionTags)) {
        return true;
      }
    }
    // Debug.Log("Max target spawn retries");
    return false;
  }

  bool SpawnRandomPrimitive() {
    PrimitiveType[] primitiveTypes = {
      PrimitiveType.Cube,
      PrimitiveType.Cube, // To increase probability
      PrimitiveType.Sphere,
      PrimitiveType.Capsule,
    };

    PrimitiveType primitiveType = primitiveTypes[Random.Range(0, primitiveTypes.Length)];
    GameObject primitive = GameObject.CreatePrimitive(primitiveType);

    primitive.transform.parent = transform;
    primitive.tag = "RandomObject";
    primitive.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());

    return RandomSpawn(primitive, randomCollisionTags, 50, primitiveType);
  }

  bool RandomSpawn(GameObject objectToSpawn, string[] collisionTags, int retries = 10, PrimitiveType? primitiveType = null) {
    for (int i = 0; i < retries; i++) {
      objectToSpawn.transform.localPosition = GetRandomPosition(env.width, 0, env.length);
      objectToSpawn.transform.localRotation = GetRandomRotation();
      objectToSpawn.transform.localScale = GetRandomScale(primitiveType);

      if (
        !Collision(
          objectToSpawn.transform,
          collisionTags,
          0.5f + (primitiveType.Equals(PrimitiveType.Cube) ? 1.415f : 1f)
        )
      ) {
        spawnedObjects.Add(objectToSpawn);
        return true;
      }
    }

    Destroy(objectToSpawn);
    return false;
  }

  bool Collision(Transform objectToCheck, string[] collisionTags, float safetyFactor = 1f) {
    Collider[] colliders = Physics.OverlapSphere(
        objectToCheck.position,
        safetyFactor * Mathf.Max(
          Mathf.Max(
            objectToCheck.localScale.x,
            objectToCheck.localScale.y
          ),
          objectToCheck.localScale.z
        )
      );

    // Debug.Log("??? Collision check for " + objectToCheck.name + " (" + objectToCheck.tag + ")");

    foreach (Collider collider in colliders) {
      bool groundCollision = collider.name.Equals("Mesh Generator");
      if (!groundCollision) {
        // Debug.Log(
        //   objectToCheck.gameObject.name + " (" + objectToCheck.tag + ") collides with " +
        //   collider.name + " (" + collider.tag + ")", collider
        // );
        foreach (string collisionTag in collisionTags) {
          if (collider.CompareTag(collisionTag)) {
            // Debug.Log("!!! COLLISION");
            return true;
          }
        }
      }
    }

    // Debug.Log("### No collision");

    return false;
  }

  Vector3 GetRandomPosition(float x = 0, float y = 0, float z = 0) {
    return new Vector3(
     Random.Range((-x / 2) * (1f - outerSafeArea), (x / 2) * (1f - outerSafeArea)),
     Random.Range((-y / 2) * (1f - outerSafeArea), (y / 2) * (1f - outerSafeArea)),
     Random.Range((-z / 2) * (1f - outerSafeArea), (z / 2) * (1f - outerSafeArea))
   );
  }

  Quaternion GetRandomRotation(float x = 360f, float y = 360f, float z = 360f) {
    return Quaternion.Euler(
     Random.Range(-x / 2, x / 2),
     Random.Range(-y / 2, y / 2),
     Random.Range(-z / 2, z / 2)
   );
  }

  Vector3 GetRandomScale(PrimitiveType? primitiveType = null) {
    if (primitiveType == PrimitiveType.Cube) {
      float minSquareSide = (minScale / Mathf.Sqrt(2f));
      float maxSquareSide = (maxScale / Mathf.Sqrt(2f));
      return new Vector3(
        Random.Range(minSquareSide, maxSquareSide),
        Random.Range(minSquareSide, maxSquareSide),
        Random.Range(minSquareSide, maxSquareSide)
      );
    } else if (primitiveType == PrimitiveType.Capsule) {
      float diameter = Random.Range(minScale, maxScale);
      return new Vector3(
        diameter / 2,
        diameter / 2,
        diameter / 2
      );
    } else {
      float equalScale = Random.Range(minScale, maxScale);
      return new Vector3(
        equalScale,
        equalScale,
        equalScale
      );
    }
  }

  void DestroySpawnedObjects(float delay = 0) {
    if (spawnedObjects != null && spawnedObjects.Count > 0) {
      foreach (GameObject spawnedObject in spawnedObjects) {
        if (runInEditMode) {
          DestroyImmediate(spawnedObject);
        } else {
          Destroy(spawnedObject, delay);
        }
      }
    }
    spawnedObjects = new List<GameObject>();
  }
}
