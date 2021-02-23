using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Overview : MonoBehaviour {
  public RandomEnvironment env;
  public VehicleAgent agent;
  public Transform background;
  public float width = 10f;
  public float height = 5f;

  private List<float> lastRewards = new List<float>(5);

  private void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();
    if (agent == null) agent = transform.parent.GetComponentInChildren<VehicleAgent>();
    if (background == null) background = GetComponentInChildren<Transform>();

    // Position overview at border (in corner?)
  }

  private void Update() {

  }
}
