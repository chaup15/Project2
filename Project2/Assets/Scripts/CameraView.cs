using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{

    [SerializeField] Transform drone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamView();
    }

    void UpdateCamView()
    {
        Vector3 targetPos = drone.position;
        transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z + 5f);

        Vector3 direction = (drone.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
