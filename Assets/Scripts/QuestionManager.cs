using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    public SubjectGuessManager subjManager;
    public TimerManager timeManager;


    [Header("UI Elements")]
    public TextMeshProUGUI questionText;  
    public Button[] problemButtons;

    private int correctIndex;

    public void GetQuestionFor(string subject)
    {
        QuestionData data = null;

        switch (subject)
        {
            case "Math":
                data = MathQuestionProvider.GetQuestion();
                break;
            case "Literature":
                data = LitQuestionProvider.GetQuestion();
                break;
            case "Science":
                data = ScienceQuestionProvider.GetQuestion();
                break;
        }

        if (data != null)
        {
            DisplayQuestion(data);

            foreach (var btn in problemButtons)
                btn.gameObject.SetActive(true);
            // SubjectGuessManager handles transitions
        }
    }

    private void DisplayQuestion(QuestionData data)
    {
        questionText.text = data.question;
        correctIndex = data.correctAnswerIndex;

        for (int i = 0; i < problemButtons.Length; i++)
        {
            int index = i;
            problemButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = data.choices[i];
            problemButtons[i].onClick.RemoveAllListeners();
            problemButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    private void OnOptionSelected(int index)
    {
        if (index == correctIndex)
        {
            Debug.Log("Correct answer!");

            timeManager.AddTime(4);

            if (subjManager.spawnedTrashcans.Count > 0)
            {
                GameObject trashcanToRemove = subjManager.spawnedTrashcans[0];
                if (trashcanToRemove != null) Destroy(trashcanToRemove);
                subjManager.spawnedTrashcans.RemoveAt(0);
            }

            StartCoroutine(TransitionManager.MoveUI(
                subjManager.problemChoicesGroup,
                subjManager.offscreenLeft, 0.5f, () =>
                {
                    subjManager.subjChoicesGroup.anchoredPosition = subjManager.problemHiddenPos;
                    StartCoroutine(TransitionManager.MoveUI(subjManager.subjChoicesGroup, subjManager.subjStartPos, 0.5f, () =>
                    {
                        subjManager.OnReturnFromProblem();
                    }));
                }));
        }
        else
        {
            Debug.Log("Wrong answer!");
        }
    }

}
