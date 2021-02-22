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

  [NonSerialized] public List<float> lastRewards = new List<float>(5);

  private void Awake() {
    if (env == null) env = GetComponentInParent<RandomEnvironment>();
    if (agent == null) agent = transform.parent.GetComponentInChildren<VehicleAgent>();
    if (background == null) background = GetComponentInChildren<Transform>();

    // Position overview at border (in corner?)
  }

  void Start() {

  }

  void Update() {
    // foreach (float reward in agent.rewards) {
    //   lastRewards.Add(reward);
    // }

    // string result = "Rewards (" + agent.rewards.Count.ToString() + "): ";

    // foreach (float reward in lastRewards) {
    //   result += reward.ToString() + ", ";
    // }

    // Debug.Log(result);
  }
}
