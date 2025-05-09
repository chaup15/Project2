using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text countdownText; 
    public Text stopwatchText;
    public Text distanceText;
    public Text checkpointText;

    private float countdownTime = 3f; 
    private float time = 0f;
    private float currentTime;
    
    public bool canMove;
    private bool finishRace;
    private bool isBegin;

    [SerializeField] GameObject drone;
    private int next;
    private GameObject[] cp;


    void Start()
    {
        currentTime = countdownTime;
        canMove = false;
        isBegin = true;
    }

    void Update()
    {
        finishRace = drone.GetComponent<CheckpointHandler>().finishRace;
        cp = GameObject.FindGameObjectsWithTag("Checkpoint");
        if(!finishRace)
        {
            if(isBegin)
            {
                Countdown();
                isBegin = false;
            }
            else
            {
                Countdown();
                
                time += Time.deltaTime;
                UpdateTimerText();
                if(next < cp.Length)
                {
                    next = drone.GetComponent<CheckpointHandler>().next;
                    float dist = Vector3.Distance(drone.transform.position, cp[next].transform.position);
                    distanceText.text = "Next: " + Mathf.Ceil(dist).ToString() + " m";
                }
                
            }
        }
        else
        {
            canMove = false;
            distanceText.text = "Next: 0 m";
        }

        checkpointText.text = next.ToString() + " / " + cp.Length.ToString() + " Checkpoints";
    }
    
    private void UpdateTimerText()
    {
        stopwatchText.text = time.ToString("F2");
    }

    private void Countdown()
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
            canMove = true;
        }
    }

    public void ResetCountdown()
    {
        currentTime = countdownTime;
    }
}
