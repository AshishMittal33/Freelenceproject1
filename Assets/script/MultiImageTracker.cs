using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageTrackerStable : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private List<GameObject> objectPrefabs;

    [Header("UI")]
    [SerializeField] private Button resetRotationButton; // assign in inspector

    private Dictionary<string, GameObject> spawnedParents = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>();

    // Interaction
    private GameObject selectedObject;
    private Vector2 lastTwoFingerPos;

    private void Awake()
    {
        foreach (var prefab in objectPrefabs)
        {
            var emptyParent = new GameObject(prefab.name + "_Parent");
            emptyParent.SetActive(false);

            var prefabInstance = Instantiate(prefab, Vector3.zero, prefab.transform.localRotation, emptyParent.transform);
            prefabInstance.transform.localPosition = Vector3.zero;
            prefabInstance.transform.localRotation = Quaternion.identity;
            prefabInstance.SetActive(true);

            spawnedParents[prefab.name] = emptyParent;
            spawnedPrefabs[prefab.name] = prefabInstance;

            // Save original rotation for reset
            originalRotations[prefabInstance] = prefabInstance.transform.localRotation;
        }

        if (resetRotationButton != null)
            resetRotationButton.onClick.AddListener(ResetSelectedObjectRotation);
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        if (resetRotationButton != null)
            resetRotationButton.onClick.RemoveListener(ResetSelectedObjectRotation);
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
            AttachObjectToImage(trackedImage);

        foreach (var trackedImage in args.updated)
            UpdateObjectState(trackedImage);

        foreach (var trackedImage in args.removed)
            DisableObject(trackedImage.referenceImage.name);
    }

    private void AttachObjectToImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        if (!spawnedParents.ContainsKey(imageName)) return;

        var parentObj = spawnedParents[imageName];
        parentObj.SetActive(true);
        parentObj.transform.SetParent(trackedImage.transform, false);
        parentObj.transform.localPosition = Vector3.zero;
        parentObj.transform.localRotation = Quaternion.identity;
    }

    private void UpdateObjectState(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        if (!spawnedParents.ContainsKey(imageName)) return;
        var parentObj = spawnedParents[imageName];

        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            parentObj.SetActive(false);
            return;
        }

        if (!parentObj.activeSelf)
            parentObj.SetActive(true);
    }

    private void DisableObject(string imageName)
    {
        if (spawnedParents.ContainsKey(imageName))
            spawnedParents[imageName].SetActive(false);
    }

    private void Update()
    {
        HandleTouchInteraction();
    }

    private void HandleTouchInteraction()
    {
        if (Input.touchCount == 0) return;

        // Single finger drag (move)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selectedObject = hit.collider.gameObject;
                }
            }
            else if (touch.phase == TouchPhase.Moved && selectedObject != null)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(
                    new Vector3(touch.position.x, touch.position.y,
                    Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));
                selectedObject.transform.position = pos;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                selectedObject = null;
            }
        }
        // Two finger horizontal rotation
        else if (Input.touchCount == 2 && selectedObject != null)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 avgPos = (t0.position + t1.position) / 2f;

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                lastTwoFingerPos = avgPos;
            }
            else if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
            {
                float deltaX = avgPos.x - lastTwoFingerPos.x;

                // Rotate around local Y only
                selectedObject.transform.Rotate(Vector3.up, -deltaX * 0.2f, Space.Self);

                lastTwoFingerPos = avgPos;
            }
        }
    }

    public void ResetSelectedObjectRotation()
    {
        if (selectedObject != null && originalRotations.ContainsKey(selectedObject))
        {
            selectedObject.transform.localRotation = originalRotations[selectedObject];
        }
    }
}
