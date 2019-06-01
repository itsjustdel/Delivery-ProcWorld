using UnityEngine;
using System.Collections;

public class WaypointSwitcher : MonoBehaviour {

	//This script raycasts for other cars and switches waypoints to avoid collision
	public bool agentEntered;
	public bool agentExited;
	public bool scan;
	public WaypointPathfinder wppf;
	private WaypointPlayer wpplayer;
	public PathFollowingController pfc;
	// Use this for initialization
	void Start () {
		wppf= transform.parent.gameObject.GetComponent<WaypointPathfinder>();
		wpplayer = transform.parent.gameObject.GetComponent<WaypointPlayer>();
		pfc = transform.parent.gameObject.GetComponent<PathFollowingController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//if trigger finds another agent
		if(agentEntered)
		{		
			//agentEntered is set true by the AgentTrigger.cs when an agent enters that gameObject's trigger
			//triggers are on their own gameobject to keep things seperated for my brains

			//since we are in this loop, tghe trigger mustn have been fired, so this script is now in control.
			//We can now disable the agentEntered flag
			agentEntered = false;
			scan = true;

		}

		if(agentExited)
		{
			FindMiddlePath();
		}
	}

	void FindMiddlePath()
	{
		//reset the flag
		agentExited = false;
		//make the pathfinding class create a central waypoint map/lane
		//wppf.MakeNewMap("Middle");
		//grab the target from the AgentBehaviour script
		Vector3 target = transform.parent.gameObject.GetComponent<AgentBehaviour>().target.transform.position;
		//clear the path previously made (the one which moves to the left of the oncoming vehicle)
		wpplayer.Path.Clear();
		//get the pathfinding class to find its way to the target again
		wpplayer.FindPath(transform.parent.position,target);
		//disable while the pathFollwer script gets new path
		pfc.enabled = false;
		//start the coroutine which waits for the path to be filled before calling the move function
		StartCoroutine("StartPathFollower");
	}

	void OnTriggerEnter(Collider other)
	{
		if (scan)
		{
			//Debug.Log("scanning");
			if(other.tag == "WaypointL" || other.tag == "WaypointR")
			{
			//	Debug.Log(other.name);
				//switch waypoints to left or right, depending on what waypoint group the trigger captured
				if (other.tag == "WaypointL")
				{
			//		wppf.MakeNewMap("Left");
				}
				else if(other.tag == "WaypointR")
				{
			//		wppf.MakeNewMap("Right");
				}

				//now force the pathfinder to find a new path to its destination
				Vector3 target = transform.parent.gameObject.GetComponent<AgentBehaviour>().target.transform.position;
				wpplayer.Path.Clear();
				wpplayer.FindPath(transform.parent.position,target);
				pfc.enabled = false;
				StartCoroutine("StartPathFollower");
				scan = false;
			}
		}
	}

	IEnumerator StartPathFollower()
	{
		//This coroutine waits until the waypoint path finder finds a succesful path and populates its list
		//Once the list is full, it then starts the UnitySteer script by calling AsignPath()
		
		while(wppf.returnPath.Count<2)
		{
			yield return new WaitForFixedUpdate();
		}
		
		pfc.pathPoints = wppf.returnPath;
		pfc.AssignPath();
		pfc.enabled = true;

		yield break;
	}
}
