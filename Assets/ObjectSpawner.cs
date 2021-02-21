// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
  // https://docs.unity3d.com/2021.1/Documentation/ScriptReference/Resources.Load.html
  [Header("Dependencies")]
  public RandomEnvironment env;
  public GameObject target;

  [Header("Random Primitive Objects")]
  public int maxAmount = 5;
  public float minScale = 1f;
  public float maxScale = 15f;

  [Range(0, 1)] float outerSafeArea = 0.25f;
  List<GameObject> spawnedObjects = new List<GameObject>();

  public void Respawn() {
    DestroySpawnedObjects();
    SpawnTarget();
    for (int i = 0; i < maxAmount * env.difficulty; i++) {
      SpawnRandomPrimitive();
    }
  }

  void SpawnTarget() {
    target.transform.position = GetRandomPosition(
      env.width * env.difficulty,
      0,
      env.length * env.difficulty
    ) + new Vector3(
      0,
      0,
      5f * (1f - env.difficulty)
    );
  }

  void SpawnRandomPrimitive() {
    PrimitiveType[] primitiveTypes = {
      PrimitiveType.Cube,
      PrimitiveType.Cube, // To increase probability
      PrimitiveType.Sphere,
      PrimitiveType.Capsule,
    };

    PrimitiveType primitiveType = primitiveTypes[Random.Range(0, primitiveTypes.Length)];
    GameObject primitive = GameObject.CreatePrimitive(primitiveType);

    primitive.tag = "RandomObject";
    primitive.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());

    string[] collisionTags = { "Vehicles", "Target" };
    RandomSpawn(primitive, collisionTags, 10, primitiveType);
  }

  bool RandomSpawn(GameObject gameObject, string[] collisionTags, int retries = 10, PrimitiveType? primitiveType = null) {
    for (int i = 0; i < retries; i++) {
      gameObject.transform.position = GetRandomPosition(env.width, 0, env.length);
      gameObject.transform.rotation = GetRandomRotation();
      gameObject.transform.localScale = GetRandomScale(primitiveType);

      if (!Collision(gameObject.transform, collisionTags)) {
        spawnedObjects.Add(gameObject);
        return true;
      }
    }

    Destroy(gameObject);
    return false;
  }

  bool Collision(Transform transform, string[] tags, float safetyFactor = 1.25f) {
    Collider[] colliders = Physics.OverlapSphere(
        transform.position,
        Mathf.Max(
          Mathf.Max(
            transform.localScale.x,
            transform.localScale.y
          ),
          transform.localScale.z
        ) * safetyFactor
      );

    foreach (Collider collider in colliders) {
      foreach (string tag in tags) {
        if (collider.tag == tag) {
          return true;
        }
      }
    }

    return false;
  }

  Vector3 GetRandomPosition(float x = 0, float y = 0, float z = 0) {
    return new Vector3(
     Random.Range((-x / 2) * (1 - outerSafeArea), (x / 2) * (1 - outerSafeArea)),
     Random.Range((-y / 2) * (1 - outerSafeArea), (y / 2) * (1 - outerSafeArea)),
     Random.Range((-z / 2) * (1 - outerSafeArea), (z / 2) * (1 - outerSafeArea))
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
