using UnityEngine;
using System.Collections;

public class WaypointsForHouses : MonoBehaviour {
	//this script is attached to the house prefab

	public bool addWaypoints;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(addWaypoints)
		{
			//grab waypoint from this object
			WaypointNode waypointNode = gameObject.GetComponent<WaypointNode>();

			//add hits as neighbours to this node
			//recreate map..call this from other script before map creation, find gameobject array


			addWaypoints = false;
		}
	}
}
