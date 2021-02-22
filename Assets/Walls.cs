using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Walls : MonoBehaviour {
  [System.NonSerialized] public RandomEnvironment env;
  float height = 2f;
  [System.NonSerialized] public GameObject wall1;
  [System.NonSerialized] public GameObject wall2;
  [System.NonSerialized] public GameObject wall3;
  [System.NonSerialized] public GameObject wall4;

  private void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();

    if (wall1 == null) wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    if (wall2 == null) wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    if (wall3 == null) wall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    if (wall4 == null) wall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);

    wall1.transform.parent = transform;
    wall1.transform.localPosition = new Vector3(0, height / 2, -0.5f + env.width / 2);
    wall1.transform.localScale = new Vector3(env.width, height, 1);

    wall2.transform.parent = transform;
    wall2.transform.localPosition = new Vector3(0, height / 2, 0.5f - env.width / 2);
    wall2.transform.localScale = new Vector3(env.width, height, 1);

    wall3.transform.parent = transform;
    wall3.transform.localPosition = new Vector3(-0.5f + env.length / 2, height / 2, 0);
    wall3.transform.localScale = new Vector3(1, height, env.length - 2);

    wall4.transform.parent = transform;
    wall4.transform.localPosition = new Vector3(0.5f - env.length / 2, height / 2, 0);
    wall4.transform.localScale = new Vector3(1, height, env.length - 2);
  }
}
