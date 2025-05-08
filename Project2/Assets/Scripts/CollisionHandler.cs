using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    Timer timer;
    [SerializeField] GameObject canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = canvas.GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider map)
    {
        Debug.Log("Collide");
        timer.ResetCountdown();
    }

}
