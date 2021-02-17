using UnityEngine;

public class clickable : MonoBehaviour
{
    public float scaleFactor = 0.1f;

    private void OnMouseEnter()
    {
        transform.localScale += scaleFactor * Vector3.one;
    }

    private void OnMouseExit()
    {
        transform.localScale -= scaleFactor * Vector3.one;
    }

    private void OnMouseDown()
    {
        transform.localScale -= 0.25f * scaleFactor * Vector3.one;
    }

    private void OnMouseUp()
    {
        transform.localScale += 0.25f * scaleFactor * Vector3.one;
    }
}
