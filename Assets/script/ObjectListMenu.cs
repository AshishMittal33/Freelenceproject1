using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectListMenu : MonoBehaviour
{
    public MultiImageTrackerStable imageTracker; // Assign via Inspector

    [Header("UI References")]
    public RectTransform contentPanel; // Scroll Rect Content panel
    public Button buttonPrefab;        // Button prefab for each object

    private List<Button> generatedButtons = new List<Button>();

    void Start()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        foreach (var btn in generatedButtons)
            Destroy(btn.gameObject);
        generatedButtons.Clear();

        foreach (var prefab in imageTracker.objectPrefabs)
        {
            var btn = Instantiate(buttonPrefab, contentPanel);
            btn.gameObject.SetActive(true);

            Text btnText = btn.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = prefab.name;

            string imageName = prefab.name;  // capture for closure

            btn.onClick.AddListener(() => OnObjectSelected(imageName));

            generatedButtons.Add(btn);
        }
    }

    private void OnObjectSelected(string imageName)
    {
        // Disable all models
        foreach (var kvp in imageTracker.spawnedParents)
            kvp.Value.SetActive(false);

        // Enable chosen model
        if (imageTracker.spawnedParents.ContainsKey(imageName))
        {
            imageTracker.spawnedParents[imageName].SetActive(true);
            imageTracker.currentlyActiveImage = imageName;
        }

        if (ARUIManager.Instance != null)
            ARUIManager.Instance.HideAllPanels();
    }
}
