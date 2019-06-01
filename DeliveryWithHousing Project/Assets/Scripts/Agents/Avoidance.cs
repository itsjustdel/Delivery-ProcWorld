using UnityEngine;
using System.Collections;

public class Avoidance : MonoBehaviour {

    public GameObject other;
    public WaypointPlayer waypointplayer;
    public Vector3 avoidanceVector = Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //direction towards target
        Vector3 direction =transform.position - other.transform.position;

        if (direction.magnitude < 5)
        {
            //   waypointplayer.speed = 0;
            avoidanceVector = direction.normalized;
            avoidanceVector *= 0.5f;
        }
        else
            avoidanceVector = Vector3.zero;
        
	}
}
