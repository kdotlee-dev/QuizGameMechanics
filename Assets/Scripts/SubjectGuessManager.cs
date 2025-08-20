using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SubjectGuessManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;                
    public Button[] choiceButtons;

    [Header("Other References")]
    public QuestionManager questionManager;

    private readonly List<string> subjects = new List<string>() { "Math", "Literature", "Science" };

    private readonly Queue<string> subjectQueue = new Queue<string>();
    private string currentTrashcanSubject;

    [Header("UI Groups (RectTransform)")]
    public RectTransform subjChoicesGroup;
    public RectTransform problemChoicesGroup;

    public Vector2 subjStartPos;                         
    public Vector2 problemHiddenPos;                   
    public Vector2 offscreenLeft = new Vector2(-2000, 0);

    [Header("Trashcan Prefabs")]
    public GameObject mathTrashcan;
    public GameObject literatureTrashcan;
    public GameObject scienceTrashcan;

    private Dictionary<string, GameObject> trashcanPrefabMap;

    [Header("Placeholders (empty scene objects, left→right)")]
    public Transform[] placeholders;

    [Header("Round Settings")]
    public int minTrashcans = 1;
    public int maxTrashcans = 6;

    public readonly List<GameObject> spawnedTrashcans = new List<GameObject>();

    private const string SubjectPrompt = "What subject is this trashcan?";

    private void Awake()
    {
        trashcanPrefabMap = new Dictionary<string, GameObject>()
        {
            { "Math",       mathTrashcan },
            { "Literature", literatureTrashcan },
            { "Science",    scienceTrashcan }
        };
    }

    private void Start()
    {
        subjStartPos = subjChoicesGroup.anchoredPosition;
        problemHiddenPos = new Vector2(subjStartPos.x, 800); 
        problemChoicesGroup.anchoredPosition = problemHiddenPos;

        StartNewRound();
    }

    public void StartNewRound()
    {
        for (int i = spawnedTrashcans.Count - 1; i >= 0; i--)
            if (spawnedTrashcans[i] != null) Destroy(spawnedTrashcans[i]);
        spawnedTrashcans.Clear();
        subjectQueue.Clear();

        questionText.text = SubjectPrompt;

        int maxAllowed = Mathf.Min(maxTrashcans, placeholders.Length);
        int count = Random.Range(minTrashcans, maxAllowed + 1);

        List<string> roundSubjects = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            string subj = subjects[Random.Range(0, subjects.Count)];
            roundSubjects.Add(subj);
            subjectQueue.Enqueue(subj);
        }

        currentTrashcanSubject = subjectQueue.Peek();

        int startIndex = ComputeStartIndex(count, placeholders.Length);
        for (int i = 0; i < count; i++)
        {
            string subj = roundSubjects[i];
            var prefab = trashcanPrefabMap[subj];

            Transform slot = placeholders[startIndex + i];

            GameObject spawned = Instantiate(prefab, slot.position, slot.rotation);

            spawned.transform.SetParent(slot, worldPositionStays: true);

            spawned.transform.localScale = Vector3.one;

            spawnedTrashcans.Add(spawned);
        }


        List<string> shuffledChoices = new List<string>(subjects);
        for (int i = 0; i < shuffledChoices.Count; i++)
        {
            int r = Random.Range(i, shuffledChoices.Count);
            (shuffledChoices[i], shuffledChoices[r]) = (shuffledChoices[r], shuffledChoices[i]);
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = shuffledChoices[idx];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnAnswerSelected(shuffledChoices[idx]));
        }

        Debug.Log("Subjects this round: " + string.Join(", ", roundSubjects));
    }

    private int ComputeStartIndex(int count, int totalSlots)
    {
        if (totalSlots == 6)
        {
            switch (count)
            {
                case 1:
                case 2:
                case 3: return 2; // slots 3–5
                case 4:
                case 5: return 1; // slots 2–5
                case 6: return 0; // slots 1–6
            }
        }
        return Mathf.Clamp((totalSlots - count) / 2, 0, totalSlots - count);
    }

    private void OnAnswerSelected(string chosen)
    {
        if (subjectQueue.Count == 0) return;

        if (chosen == currentTrashcanSubject)
        {
            string solvedSubject = currentTrashcanSubject;

            subjectQueue.Dequeue();

            if (subjectQueue.Count > 0)
                currentTrashcanSubject = subjectQueue.Peek();

            StartCoroutine(TransitionManager.MoveUI(
                subjChoicesGroup, offscreenLeft, 0.5f, () =>
                {
                    problemChoicesGroup.anchoredPosition = problemHiddenPos;
                    StartCoroutine(TransitionManager.MoveUI(problemChoicesGroup, subjStartPos, 0.5f));
                }));

            questionManager.GetQuestionFor(solvedSubject);
        }
        else
        {
            Debug.Log("Wrong!");
        }
    }


    public void OnReturnFromProblem()
    {
        if (subjectQueue.Count > 0)
        {
            questionText.text = SubjectPrompt;
        }
        else
        {
            StartNewRound();
        }
    }
}
