using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTester : MonoBehaviour {

    public WaypointPathfinder waypointPathfinder;public GameObject start;
    public GameObject end;
    public List<Vector3> path;
    public bool test = false;
    // Use this for initialization
    void Start ()
    {
		
	}
    private void Update()
    {
        if(test)
        {
            Go();
            test = false;
        }

    }

    void Go()
    {
        path = waypointPathfinder.FindPath(start.transform.position, end.transform.position);

        foreach(Vector3 v3 in path)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = v3;
        }


    }


}
