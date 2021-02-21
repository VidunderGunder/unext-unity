using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour {
  public RandomEnvironment env;
  float height = 2f;

  void Start() {
    GameObject wall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall1.transform.position = new Vector3(0, height / 2, -0.5f + env.width / 2);
    wall1.transform.localScale = new Vector3(env.width, height, 1);

    GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall2.transform.position = new Vector3(0, height / 2, 0.5f - env.width / 2);
    wall2.transform.localScale = new Vector3(env.width, height, 1);

    GameObject wall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall3.transform.position = new Vector3(-0.5f + env.length / 2, height / 2, 0);
    wall3.transform.localScale = new Vector3(1, height, env.length - 2);

    GameObject wall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    wall4.transform.position = new Vector3(0.5f - env.length / 2, height / 2, 0);
    wall4.transform.localScale = new Vector3(1, height, env.length - 2);
  }
}
