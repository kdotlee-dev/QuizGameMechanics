using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public Image timerCountdownImage;
    public int max = 20;
    private float currentTime;

    void Start()
    {
        currentTime = max;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        timerCountdownImage.fillAmount = currentTime / max;
    }

    public void AddTime(int amount = 3)
    {
        currentTime += amount;

        if (currentTime > max)
            currentTime = max;

        UpdateTimerUI();
    }
}
