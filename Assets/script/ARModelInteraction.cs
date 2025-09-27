using UnityEngine;

public class ARModelInteraction : MonoBehaviour
{
    private bool isSelected = false;

    // Jab object select ho jaaye to call karna
    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    void Update()
    {
        if (!isSelected) return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float rotateSpeed = 0.2f;
                float rotY = -touch.deltaPosition.x * rotateSpeed;
                transform.Rotate(0, rotY, 0, Space.Self);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            Vector2 avgDelta = (t0.deltaPosition + t1.deltaPosition) * 0.5f;
            float moveSpeed = 0.001f;
            transform.Translate(avgDelta.x * moveSpeed, 0, avgDelta.y * moveSpeed, Space.Self);
        }
    }
}
