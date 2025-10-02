using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "ARData/QuizData")]
public class QuizDataSO : ScriptableObject
{
    public string question;
    public string[] options = new string[4]; // 4 answer options
    public int correctOptionIndex; // Index of the correct answer (0 to 3)
}
