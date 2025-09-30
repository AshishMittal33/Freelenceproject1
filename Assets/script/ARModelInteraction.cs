using UnityEngine;

public class ARModelInteraction : MonoBehaviour
{
    private bool isSelected = false;
    private ARModelInfoHolder infoHolder;

    [Header("Highlight Object")]
    [SerializeField] private GameObject highlightObject; // Drag your cube child here
    public bool IsSelected => isSelected;

    private void Awake()
    {
        infoHolder = GetComponent<ARModelInfoHolder>();

        // If highlight object is not assigned, try to find it automatically
        if (highlightObject == null)
        {
            highlightObject = transform.Find("highlight")?.gameObject;
            // OR find by tag
            // highlightObject = GameObject.FindGameObjectWithTag("Highlight");
        }

        if (highlightObject != null)
            highlightObject.SetActive(false);
        else
            Debug.LogError("Highlight Object not found on: " + gameObject.name);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        // Toggle the highlight object
        if (highlightObject != null)
            highlightObject.SetActive(selected);

        if (selected && infoHolder != null)
        {
            ARUIManager.Instance.ShowInfoPanel(infoHolder.objectInfo);
        }
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