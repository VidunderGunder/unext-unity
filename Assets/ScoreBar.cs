using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ScoreBar : MonoBehaviour {
  public VehicleAgent agent;
  public RectTransform rewardFill;
  public RectTransform penaltyFill;
  public int episodesBack = 0;

  [Range(-1f, 1f)] private float score = 0;
  private float maxValue;

  // Start is called before the first frame update
  void Start() {
    rewardFill.anchorMax.Set(score > 0 ? score : 0, rewardFill.anchorMax.y);
    penaltyFill.anchorMin.Set(score < 0 ? -score : 0, penaltyFill.anchorMin.y);
    maxValue = Mathf.Max(-agent.minCumulativeReward, agent.maxCumulativeReward);
  }

  // Update is called once per frame
  void Update() {
    if (Application.isPlaying) {
      if (episodesBack > 0 & agent.rewards.Count >= episodesBack) {
        score = agent.rewards[agent.rewards.Count - episodesBack];
        score /= maxValue;
      } else if (episodesBack == 0) {
        score = agent.GetCumulativeReward();
        score /= maxValue;
      }
    } else {
      score = Random.Range(-1f, 1f);
      DrawScore(score);
    }
    DrawScore(score);
  }

  private void DrawScore(float score) {
    rewardFill.anchorMax = new Vector2(score > 0 ? score : 0, rewardFill.anchorMax.y);
    penaltyFill.anchorMin = new Vector2(score < 0 ? 1f + score : 1f, penaltyFill.anchorMin.y);
  }
}
