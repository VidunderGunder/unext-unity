using UnityEngine;

[ExecuteInEditMode]
public class RandomEnvironment : MonoBehaviour {
  public int width;
  public int length;
  [Range(0, 1f)] public float difficulty = 0;

  void Awake() {
    // Force even dimensions
    // (Odd numbers causes glitches in the ground mesh)
    if (width % 2 != 0) {
      width -= 1;
    }
    if (length % 2 != 0) {
      length -= 1;
    }
  }
}
