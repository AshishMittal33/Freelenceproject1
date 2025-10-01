using UnityEngine;
using System.Collections.Generic;

public class ARLabelManager : MonoBehaviour
{
    public ObjectLabelsSO labelsData;        // Assign in Inspector
    public GameObject labelPrefab;           // Prefab with TMP + Button
    public Transform labelsParent;           // Empty child for organizing labels

    private List<GameObject> spawnedLabels = new List<GameObject>();

    private bool labelsVisible = false;

    void Start()
    {
        SpawnLabels();
        ToggleLabels(false); // Start hidden
    }

    void SpawnLabels()
    {
        foreach (var label in labelsData.labels)
        {
            GameObject newLabel = Instantiate(labelPrefab, labelsParent);
            newLabel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = label.labelName;

            // Position relative to model
            newLabel.transform.localPosition = label.localPosition;

            // Capture local copy
            string info = label.labelInfo;
            newLabel.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                ARUIManager.Instance.ShowInfoPanel(new ObjectInfoSO { infoText = info });
            });

            spawnedLabels.Add(newLabel);
        }
    }

    public void ToggleLabels(bool show)
    {
        labelsVisible = show;
        foreach (var lbl in spawnedLabels)
            lbl.SetActive(show);
    }

    // Helper toggle function
    public void ToggleLabels()
    {
        ToggleLabels(!labelsVisible);
    }
}
