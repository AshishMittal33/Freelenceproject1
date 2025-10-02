using UnityEngine;

[System.Serializable]
public class ObjectLabel
{
    public string labelName;           // e.g., "Eyes"
    [TextArea] public string labelInfo; // Info about this part
    public Vector3 localPosition;       // Position relative to the object
}

[CreateAssetMenu(fileName = "ObjectLabels", menuName = "AR/Object Labels")]
public class ObjectLabelsSO : ScriptableObject
{
    public ObjectLabel[] labels;
}
