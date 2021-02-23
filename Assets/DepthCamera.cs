using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthCamera : MonoBehaviour {
  public Camera cam;
  public Material mat;

  void Start() {
    if (cam == null) {
      cam = this.GetComponent<Camera>();
    }
    cam.depthTextureMode = DepthTextureMode.DepthNormals;

    if (mat == null) {
      mat = new Material(Shader.Find("Hidden/DepthCamera"));
    }
  }

  private void OnPreRender() {
    Shader.SetGlobalMatrix(Shader.PropertyToID("UNITY_MATRIX_IV"), cam.cameraToWorldMatrix);
  }

  private void OnRenderImage(RenderTexture src, RenderTexture dest) {
    Graphics.Blit(src, dest, mat);
  }
}
