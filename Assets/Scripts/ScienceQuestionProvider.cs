using UnityEngine;
using System.Collections.Generic;

public static class ScienceQuestionProvider
{
    //1st str question
    //2nd string answer
    //3rd string multiple choices(include the correct answer, the other 2 random but related)
    private static List<(string, string, string[])> questions = new List<(string, string, string[])>
    {
        ("Which animal is a mammal?", "Mammoth", new string[] { "Crocodile", "Mammoth", "Snake" }),
        ("Which planet is known as the Red Planet?", "Mars", new string[] { "Mars", "Venus", "Jupiter" }),
        ("What gas do humans need to breathe?", "Oxygen", new string[] { "Oxygen", "Carbon Dioxide", "Nitrogen" })
    };

    public static QuestionData GetQuestion()
    {
        var q = questions[Random.Range(0, questions.Count)];
        string question = q.Item1;
        string correctAnswer = q.Item2;
        string[] allChoices = q.Item3;

        List<string> choiceList = new List<string>(allChoices);
        Shuffle(choiceList);

        int correctIndex = choiceList.IndexOf(correctAnswer);

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
