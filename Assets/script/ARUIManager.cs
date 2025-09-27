using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARUIManager : MonoBehaviour
{
    public static ARUIManager Instance;

    [Header("Info Panel UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;
    public Button quizButton;

    [Header("Quiz Panel UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI quizQuestionText;
    public Button[] optionButtons; // Array of 4 buttons for answers
    public TextMeshProUGUI feedbackText;
    public Button exitQuizButton;

    private QuizDataSO currentQuiz;

    void Awake()
    {
        Instance = this;

        // Register button event listeners
        quizButton.onClick.AddListener(ShowQuizPanel);
        exitQuizButton.onClick.AddListener(HideAllPanels);
    }

    // Show the info panel with data from ScriptableObject
    public void ShowInfoPanel(ObjectInfoSO objectInfo)
    {
        infoPanel.SetActive(true);
        quizPanel.SetActive(false);

        infoText.text = objectInfo.infoText;
        currentQuiz = objectInfo.quizData;

        // Set quiz answer options text and button click handlers
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // Capture local copy for lambda
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuiz.options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        feedbackText.text = ""; // Clear feedback text initially
    }

    private void ShowQuizPanel()
    {
        infoPanel.SetActive(false);
        quizPanel.SetActive(true);
        quizQuestionText.text = currentQuiz.question;
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (selectedIndex == currentQuiz.correctOptionIndex)
        {
            feedbackText.text = "Congratulations! Correct answer.";
        }
        else
        {
            feedbackText.text = $"Wrong answer. Correct: {currentQuiz.options[currentQuiz.correctOptionIndex]}";
        }
        // Hide all UI after 3 seconds and return to AR
        Invoke(nameof(HideAllPanels), 3f);
    }

    public void HideAllPanels()
    {
        infoPanel.SetActive(false);
        quizPanel.SetActive(false);
        feedbackText.text = "";
    }
}
