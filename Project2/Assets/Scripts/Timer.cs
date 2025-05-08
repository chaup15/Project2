using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text countdownText; 
    public Text stopwatchText;
    private float countdownTime = 3f; 
    private float time = 0f;
    private float currentTime;
    public bool canMove;

    void Start()
    {
        currentTime = countdownTime;
        canMove = false;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            canMove = false;
            currentTime -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(currentTime).ToString();
        }
        else
        {
            countdownText.text = "";
            time += Time.deltaTime;
            UpdateTimerText();
            canMove = true;
        }
    }
    
    private void UpdateTimerText()
    {
        stopwatchText.text = time.ToString("F2");
    }

    public void ResetCountdown()
    {
        currentTime = countdownTime;
    }
}
