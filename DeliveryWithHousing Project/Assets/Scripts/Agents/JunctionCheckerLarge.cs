using UnityEngine;
using System.Collections;

public class JunctionCheckerLarge : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Agent")
		{
			other.transform.Find("LaneSwitcher").GetComponent<WaypointSwitcher>().scan = true;
			Debug.Log("switching lanes at junction");
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "AgentJunctionTrigger")
		{
			//other.transform.FindChild("LaneSwitcher").GetComponent<WaypointSwitcher>().agentExited = true;
			//Debug.Log("switching lanes at junction");
		}
	}
}
