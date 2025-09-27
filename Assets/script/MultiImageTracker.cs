using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageTrackerStable : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] internal List<GameObject> objectPrefabs;

    public Dictionary<string, GameObject> spawnedParents = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    public string currentlyActiveImage = null;

    private void Awake()
    {
        foreach (var prefab in objectPrefabs)
        {
            // Empty parent to reset rotation
            var emptyParent = new GameObject(prefab.name + "_Parent");
            emptyParent.SetActive(false);

            // Instantiate prefab as child of empty
            var prefabInstance = Instantiate(prefab, Vector3.zero, prefab.transform.localRotation, emptyParent.transform);
            prefabInstance.transform.localPosition = Vector3.zero;
            prefabInstance.transform.localRotation = Quaternion.identity; // Nullify inner prefab rotation, handled by parent
            prefabInstance.SetActive(true);

            spawnedParents[prefab.name] = emptyParent;
            spawnedPrefabs[prefab.name] = prefabInstance;
        }
    }
    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (var obj in FindObjectsOfType<ARModelInteraction>())
                    obj.SetSelected(false);

                ARModelInteraction interact = hit.transform.GetComponentInParent<ARModelInteraction>();
                if (interact != null)
                    interact.SetSelected(true);
            }
        }
    }


    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
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

        // If a different image is already active, disable its model
        if (currentlyActiveImage != null && currentlyActiveImage != imageName)
        {
            DisableObject(currentlyActiveImage);
        }

        currentlyActiveImage = imageName;

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

        // Disable when not tracking
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            parentObj.SetActive(false);
            return;
        }

        if (!parentObj.activeSelf)
            parentObj.SetActive(true);

        // Optional: Smoothing! (prevents jitter)
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.identity;
        parentObj.transform.localPosition = Vector3.Lerp(parentObj.transform.localPosition, targetPos, 0.5f);
        parentObj.transform.localRotation = Quaternion.Slerp(parentObj.transform.localRotation, targetRot, 0.5f);
    }

    private void DisableObject(string imageName)
    {
        if (spawnedParents.ContainsKey(imageName))
        {
            spawnedParents[imageName].SetActive(false);

            if (currentlyActiveImage == imageName)
                currentlyActiveImage = null;

            // Hide UI when model hidden
            if (ARUIManager.Instance != null)
                ARUIManager.Instance.HideAllPanels();
        }
    }


}
