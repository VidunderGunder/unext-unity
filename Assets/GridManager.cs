using UnityEngine;

public class GridManager : MonoBehaviour {
  public float xStart, yStart, zStart;
  public int xInstances, yInstances, zInstances;
  public GameObject prefab;
  public float margin = 5f;

  [System.NonSerialized] public float xDistance, yDistance, zDistance;
  public RandomEnvironment env;

  void Awake() {
    if (env == null) env = transform.parent.Find("RandomEnvironment").GetComponent<RandomEnvironment>();
    xDistance = env.width + margin;
    yDistance = 50;
    zDistance = env.length + margin;
  }

  void Start() {
    for (int x = 0; x < xInstances; x++) {
      for (int z = 0; z < zInstances; z++) {
        for (int y = 0; y < yInstances; y++) {
          if (!(x.Equals(0) & y.Equals(0) & z.Equals(0))) {
            Instantiate(prefab, new Vector3(
                xStart + (x * xDistance),
                yStart + (y * yDistance),
                zStart + (z * zDistance)
            ), Quaternion.identity);
          }
        }
      }
    }
  }

  void Update() {

  }
}