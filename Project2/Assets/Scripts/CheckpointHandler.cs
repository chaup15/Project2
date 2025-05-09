using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHandler : MonoBehaviour
{
    Timer timer;
    [SerializeField] GameObject canvas;

    [SerializeField] GameObject list;
    [SerializeField] Transform cam;

    private List<Vector3> cp;
    private int prev;
    public int next;
    private float scaleFactor;
    private LineRenderer line;
    public bool finishRace;

    // Start is called before the first frame update
    void Start()
    {
        timer = canvas.GetComponent<Timer>();
        cp = list.GetComponent<Parse>().pointsPos;
        prev = 0;
        next = 1;
        scaleFactor = 1f/3.28f;
        finishRace = false;

        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    // Update is called once per frame
    void Update()
    {
        line.enabled = true;
        cp = list.GetComponent<Parse>().pointsPos;
        if(next == cp.Count)
        {
            finishRace = true;
            line.enabled = false;
        }
        else
        {
            PassCheckpoint();
            if(next < cp.Count)
            {
                line.SetPosition(0, cam.position);
                line.SetPosition(1, cp[next]);
                line.startColor = Color.yellow;
                line.endColor = Color.yellow;
            }
        }
    }

    void OnTriggerEnter(Collider map)
    {
        timer.ResetCountdown();
        if(cp.Count > 0)
            ResetDronePos();
    }

    void ResetDronePos()
    {
        transform.position = cp[prev];
    }

    void PassCheckpoint()
    {
        if(Vector3.Distance(transform.position, cp[next]) < scaleFactor * 30)
        {
            prev++;
            next++;
        }
    }
}
