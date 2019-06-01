using UnityEngine;
using System.Collections;

public class StopForCarInFront : MonoBehaviour {

    public WaypointPlayer waypointPlayer;
    public bool stoppedForCarInFront;
	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		//
		if(other.tag == "Agent")
		{
            waypointPlayer.speed = 0;
            stoppedForCarInFront = true;
		//	Debug.Log("entered");
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		//
		if(other.tag == "Agent")
		{
            waypointPlayer.speed = 5;
            stoppedForCarInFront = false;
        }
	}

}
