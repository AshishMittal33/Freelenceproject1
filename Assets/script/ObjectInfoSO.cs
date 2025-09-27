using UnityEngine;

[CreateAssetMenu(fileName = "ObjectInfo", menuName = "ARData/ObjectInfo")]
public class ObjectInfoSO : ScriptableObject
{
    [TextArea] public string infoText; // Info about the object
    public QuizDataSO quizData; // Reference to quiz data
}
