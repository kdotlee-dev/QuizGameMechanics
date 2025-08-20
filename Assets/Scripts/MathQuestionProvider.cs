using UnityEngine;
using System.Collections.Generic;

public static class MathQuestionProvider
{
    public static QuestionData GetQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int correctAnswer = a + b;

        string question = $"What is {a} + {b}?";

        HashSet<int> options = new HashSet<int> { correctAnswer };
        while (options.Count < 3)
        {
            options.Add(correctAnswer + Random.Range(-3, 4));
        }

        List<string> choiceList = new List<string>();
        foreach (int val in options)
            choiceList.Add(val.ToString());

        Shuffle(choiceList);

        int correctIndex = choiceList.IndexOf(correctAnswer.ToString());

        return new QuestionData
        {
            question = question,
            choices = choiceList.ToArray(),
            correctAnswerIndex = correctIndex
        };
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
