using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Walls : MonoBehaviour {
  private RandomEnvironment env;
  float height = 2f;
  private Transform wall1;
  private Transform wall2;
  private Transform wall3;
  private Transform wall4;

  private void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();

    if (wall1 == null) wall1 = transform.Find("Wall 1");
    if (wall2 == null) wall2 = transform.Find("Wall 2");
    if (wall3 == null) wall3 = transform.Find("Wall 3");
    if (wall4 == null) wall4 = transform.Find("Wall 4");

    wall1.parent = transform;
    wall1.localPosition = new Vector3(0, height / 2, -0.5f + env.width / 2);
    wall1.localScale = new Vector3(env.width, height, 1);

    wall2.parent = transform;
    wall2.localPosition = new Vector3(0, height / 2, 0.5f - env.width / 2);
    wall2.localScale = new Vector3(env.width, height, 1);

    wall3.parent = transform;
    wall3.localPosition = new Vector3(-0.5f + env.length / 2, height / 2, 0);
    wall3.localScale = new Vector3(1, height, env.length - 2);

    wall4.parent = transform;
    wall4.localPosition = new Vector3(0.5f - env.length / 2, height / 2, 0);
    wall4.localScale = new Vector3(1, height, env.length - 2);
  }
}
