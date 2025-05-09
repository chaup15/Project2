using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parse : MonoBehaviour
{
    public TextAsset file;
    public GameObject checkpoint;
    public Transform xrRig;
    private LineRenderer line;

    public List<Vector3> pointsPos;
    
    // Start is called before the first frame update
    void Start()
    {
        pointsPos = ParseFile();

        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = pointsPos.Count;
        line.material = new Material(Shader.Find("Sprites/Default"));

        for(int i = 0; i < pointsPos.Count; i++)
        {
            line.SetPosition(i, pointsPos[i]);
            GameObject cp = Instantiate(checkpoint, pointsPos[i], Quaternion.identity);
            Renderer rend = cp.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.red;
            }
            
        }
        xrRig.position = pointsPos[0];
        Vector3 direction = (pointsPos[1] - pointsPos[0]).normalized;
        xrRig.rotation = Quaternion.LookRotation(direction);
    }

    // Update is called once per frame
    void Update()
    {
        line.enabled = true;
        line.startColor = Color.white;
        line.endColor = Color.white;
    }

    List<Vector3> ParseFile()
    {
        float ScaleFactor = 1.0f / 39.37f;
        List<Vector3> positions = new List<Vector3>();
        string content = file.ToString();
        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] coords = lines[i].Split(' ');
            Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
            positions.Add(pos * ScaleFactor);
        }
        return positions;
    }
}
